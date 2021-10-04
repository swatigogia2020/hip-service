using System;
using System.Threading.Tasks;
using System.Web;
using In.ProjectEKA.HipService.Common;
using In.ProjectEKA.HipService.Consent;
using In.ProjectEKA.HipService.Discovery;
using In.ProjectEKA.HipService.Link;
using In.ProjectEKA.HipService.Logger;
using In.ProjectEKA.HipService.OpenMrs;
using In.ProjectEKA.HipService.Patient.Model;
using In.ProjectEKA.HipService.UserAuth;
using Action = In.ProjectEKA.HipService.Patient.Model.Action;

namespace In.ProjectEKA.HipService.Patient
{
    public class PatientNotificationService : IPatientNotificationService
    {
        private readonly IUserAuthRepository userAuthRepository;
        private readonly ILinkPatientRepository linkPatientRepository;
        private readonly IDiscoveryRequestRepository discoveryRequestRepository;
        private readonly IConsentRepository consentRepository;
        private readonly IOpenMrsClient openMrsClient;

        public PatientNotificationService(IUserAuthRepository userAuthRepository, IOpenMrsClient openMrsClient,
            ILinkPatientRepository linkPatientRepository, IDiscoveryRequestRepository discoveryRequestRepository, IConsentRepository consentRepository)
        {
            this.userAuthRepository = userAuthRepository;
            this.openMrsClient = openMrsClient;
            this.linkPatientRepository = linkPatientRepository;
            this.discoveryRequestRepository = discoveryRequestRepository;
            this.consentRepository = consentRepository;
        }

        public async Task Perform(HipPatientStatusNotification hipPatientStatusNotification)
        {
            var healthId = hipPatientStatusNotification.notification.patient.id;
            var status = hipPatientStatusNotification.notification.status.ToString();
            if (status.Equals(Action.DELETED.ToString()))
            {
                DeleteHealthIdInHip(healthId);
                await RemoveHealthIdInOpenMrs(healthId, status);
            }
            if(status.Equals(Action.DEACTIVATED.ToString()))
            {
                await RemoveHealthIdInOpenMrs(healthId, status);
            }
        }

        private void DeleteHealthIdInHip(string healthId)
        {
            discoveryRequestRepository.DeleteDiscoveryRequest(healthId);
            linkPatientRepository.DeleteLinkedAccounts(healthId);
            linkPatientRepository.DeleteLinkEnquires(healthId);
            userAuthRepository.Delete(healthId);
            userAuthRepository.DeleteDemographics(healthId);
            consentRepository.DeleteConsentArtefact(healthId);
        }

        private async Task RemoveHealthIdInOpenMrs(string healthId, string status)
        {
            var path = $"{Constants.PATH_OPENMRS_HITYPE}existingPatients/status";
            var query = HttpUtility.ParseQueryString(string.Empty);
            if (
                !string.IsNullOrEmpty(healthId) &&
                !string.IsNullOrEmpty(status)
            )
            {
                query["healthId"] = healthId;
                query["action"] = status;
            }

            if (query.ToString() != "")
            {
                path = $"{path}?{query}";
            }

            Log.Information("endpoint being called: " + path);
            var response = await openMrsClient.GetAsync(path);
            var content = await response.Content.ReadAsStringAsync();
            Log.Information(content);
        }
    }
}