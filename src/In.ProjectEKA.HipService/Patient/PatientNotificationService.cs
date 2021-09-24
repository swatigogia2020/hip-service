using System.Threading.Tasks;
using System.Web;
using In.ProjectEKA.HipService.Common;
using In.ProjectEKA.HipService.Logger;
using In.ProjectEKA.HipService.OpenMrs;
using In.ProjectEKA.HipService.Patient.Model;
using In.ProjectEKA.HipService.UserAuth;
using Microsoft.Extensions.Logging;

namespace In.ProjectEKA.HipService.Patient
{
    public class PatientNotificationService : IPatientNotificationService
    {
        private readonly IUserAuthRepository userAuthRepository;
        private readonly IOpenMrsClient openMrsClient;

        public PatientNotificationService(IUserAuthRepository userAuthRepository, IOpenMrsClient openMrsClient,
            ILogger<PatientNotificationService> logger)
        {
            this.userAuthRepository = userAuthRepository;
            this.openMrsClient = openMrsClient;
        }

        public async Task Perform(HipPatientStatusNotification hipPatientStatusNotification)
        {
            if (hipPatientStatusNotification.notification.status.ToString().Equals(Action.DELETED.ToString()))
            {
                var healthId = hipPatientStatusNotification.notification.patient.id;
                var status = hipPatientStatusNotification.notification.status.ToString();
                DeleteHealthId(healthId);
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