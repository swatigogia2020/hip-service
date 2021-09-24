using In.ProjectEKA.HipService.Logger;
using Microsoft.AspNetCore.Http;

namespace In.ProjectEKA.HipService.Patient
{
    using System;
    using System.Threading.Tasks;
    using Gateway;
    using Hangfire;
    using HipLibrary.Patient.Model;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Model;
    using Newtonsoft.Json.Linq;
    using static Common.Constants;

    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IBackgroundJobClient backgroundJob;
        private readonly GatewayClient gatewayClient;
        private readonly ILogger<PatientController> logger;
        private readonly IPatientNotificationService patientNotificationService;
        private readonly GatewayConfiguration gatewayConfiguration;

        public PatientController(IBackgroundJobClient backgroundJob, GatewayClient gatewayClient,
            ILogger<PatientController> logger, IPatientNotificationService patientNotificationService,
            GatewayConfiguration gatewayConfiguration)
        {
            this.backgroundJob = backgroundJob;
            this.gatewayClient = gatewayClient;
            this.logger = logger;
            this.patientNotificationService = patientNotificationService;
            this.gatewayConfiguration = gatewayConfiguration;
        }

        [HttpPost(PATH_PATIENT_PROFILE_SHARE)]
        public AcceptedResult PatientProfile(
            [FromHeader(Name = CORRELATION_ID)] string correlationId,
            JObject request)
        {
            backgroundJob.Enqueue(() => ShareResponseFor(request,correlationId));
            return Accepted();
        }
        
        [NonAction]
        public async Task ShareResponseFor(JObject request, String correlationId)
        {
            var patientProfileRequest = request.ToObject<PatientProfile>();
            logger.LogInformation("Patient Details: {@PatientProfile}", patientProfileRequest);
            logger.LogInformation($"Patient Details: {patientProfileRequest.PatientDetails.UserDemographics.HealthId}");
            var cmSuffix = patientProfileRequest.PatientDetails.UserDemographics.HealthId.Substring(
                patientProfileRequest.PatientDetails.UserDemographics.HealthId.LastIndexOf("@", StringComparison.Ordinal) + 1);
            var gatewayResponse = new PatientProfileAcknowledgementResponse(
                Guid.NewGuid(),
                DateTime.Now.ToUniversalTime(),
                new Acknowledgement(patientProfileRequest.PatientDetails.UserDemographics.HealthId, Status.SUCCESS),
                new Resp(patientProfileRequest.RequestId),
                null);
            await gatewayClient.SendDataToGateway(PATH_PATIENT_PROFILE_ON_SHARE,
                gatewayResponse,
                cmSuffix,
                correlationId);
        }

        [Route(PATH_PATIENT_NOTIFY)]
        public async Task<AcceptedResult> NotifyHip([FromHeader(Name = CORRELATION_ID)] string correlationId,
            [FromBody] HipPatientStatusNotification hipPatientStatusNotification)
        {
            var cmSuffix = gatewayConfiguration.CmSuffix;
            await patientNotificationService.Perform(hipPatientStatusNotification);
            var gatewayResponse = new HipPatientNotifyConfirmation(
                Guid.NewGuid().ToString(),
                DateTime.Now.ToUniversalTime(),
                new PatientNotifyAcknowledgement(Status.SUCCESS.ToString()), null,
                new Resp(hipPatientStatusNotification.requestId.ToString()));
            await gatewayClient.SendDataToGateway(PATH_PATIENT_ON_NOTIFY,
                gatewayResponse,
                cmSuffix,
                correlationId);
            return Accepted();
        }

        [HttpPost(PATH_PATIENT_ON_NOTIFY)]
        public AcceptedResult PatientOnNotify(HipPatientNotifyConfirmation confirmation)
        {
            Log.Information("on-notify of patient received." +
                            $" RequestId:{confirmation.RequestId}, " +
                            $" Timestamp:{confirmation.Timestamp}");
            if (confirmation.Error != null)
                Log.Information($" Error Code:{confirmation.Error.Code}," +
                                $" Error Message:{confirmation.Error.Message}");
            else if (confirmation.Acknowledgement != null)
                Log.Information($" Acknowledgment Status:{confirmation.Acknowledgement.Status}");
            Log.Information($" Resp RequestId:{confirmation.Resp.RequestId}");
            return Accepted();
        }
    }
}