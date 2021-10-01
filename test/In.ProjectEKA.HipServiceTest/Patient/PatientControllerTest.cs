using In.ProjectEKA.HipService.Patient;
using In.ProjectEKA.HipService.Patient.Model;
using Task = System.Threading.Tasks.Task;
using System;
using Hangfire;
using Hl7.Fhir.Model;
using In.ProjectEKA.HipLibrary.Patient.Model;
using In.ProjectEKA.HipService.Common;
using In.ProjectEKA.HipService.Gateway;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace In.ProjectEKA.HipServiceTest.Patient
{
    using static Constants;

    public class PatientControllerTest
    {
        private readonly PatientController patientController;

        private readonly Mock<IPatientNotificationService> patientNotificationService =
            new Mock<IPatientNotificationService>();

        private readonly Mock<GatewayClient> gatewayClient = new Mock<GatewayClient>(MockBehavior.Loose, null, null);

        private readonly Mock<ILogger<PatientController>> logger =
            new Mock<ILogger<PatientController>>();

        private readonly Mock<IBackgroundJobClient> backgroundJobClient = new Mock<IBackgroundJobClient>();

        private readonly GatewayConfiguration gatewayConfiguration = new GatewayConfiguration()
        {
            CmSuffix = "sbx"
        };


        public PatientControllerTest()
        {
            patientController = new PatientController(backgroundJobClient.Object, gatewayClient.Object,
                logger.Object, patientNotificationService.Object, gatewayConfiguration);
        }


        [Fact]
        private void ShouldNotifyHip()
        {
            var requestId = Guid.NewGuid();
            var timestamp = DateTime.Now.ToUniversalTime();
            var patient = new HipNotifyPatient("test@sbx");
            var notification = new PatientNotification(HipService.Patient.Model.Action.DELETED, patient);
            var hipPatientStatusNotification = new HipPatientStatusNotification(requestId, timestamp, notification);
            var correlationId = Uuid.Generate().ToString();
            var cmSuffix = "ncg";
            var hipPatientNotifyConfirmation = new HipPatientNotifyConfirmation(Guid.NewGuid().ToString(), timestamp,
                new PatientNotifyAcknowledgement(Status.SUCCESS.ToString()),
                null, new Resp(requestId.ToString()));
            gatewayClient.Setup(
                    client =>
                        client.SendDataToGateway(PATH_PATIENT_ON_NOTIFY,
                            hipPatientNotifyConfirmation, cmSuffix, correlationId))
                .Returns(Task.FromResult(""));
            Assert.Equal(patientController.NotifyHip(correlationId, hipPatientStatusNotification).Result.StatusCode,
                StatusCodes.Status202Accepted);
        }
    }
}