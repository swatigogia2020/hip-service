using System;
using System.Threading.Tasks;
using System.Web;
using In.ProjectEKA.HipService.Common;
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
        private readonly IOpenMrsClient openMrsClient;

        public PatientNotificationService(IUserAuthRepository userAuthRepository, IOpenMrsClient openMrsClient,
            ILinkPatientRepository linkPatientRepository)
        {
            this.userAuthRepository = userAuthRepository;
            this.openMrsClient = openMrsClient;
            this.linkPatientRepository = linkPatientRepository;
        }

        public async Task Perform(HipPatientStatusNotification hipPatientStatusNotification)
        {
            if (hipPatientStatusNotification.notification.status.ToString().Equals(Action.DELETED.ToString()))
            {
                var healthId = hipPatientStatusNotification.notification.patient.id;
                var status = hipPatientStatusNotification.notification.status.ToString();

                DeleteHealthId(healthId);
                await linkPatientRepository.DeleteLinkedAccounts(healthId);
                await linkPatientRepository.DeleteLinkEnquires(healthId);
                await RemoveHealthIdFromOpenMrs(healthId, status);
            }
        }

        private void DeleteHealthId(string healthId)
        {
            userAuthRepository.Delete(healthId);
            userAuthRepository.DeleteDemographics(healthId);
        }

        private async Task RemoveHealthIdFromOpenMrs(string healthId, string status)
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