using FluentAssertions;
using Hl7.Fhir.Model;
using In.ProjectEKA.HipLibrary.Patient.Model;
using In.ProjectEKA.HipService.Common;
using In.ProjectEKA.HipService.Gateway;
using In.ProjectEKA.HipService.Linkage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace In.ProjectEKA.HipServiceTest.Linkage
{
    using System;
    using System.Threading.Tasks;
    using Moq;
    using Xunit;

    public class AuthConfirmControllerTest
    {
        private readonly AuthConfirmController authConfirmController;

        private readonly Mock<ILogger<AuthConfirmController>> logger =
            new Mock<ILogger<AuthConfirmController>>();

        private readonly Mock<GatewayClient> gatewayClient = new Mock<GatewayClient>(MockBehavior.Strict, null, null);
        private readonly Mock<AuthConfirmService> authConfirmService = new Mock<AuthConfirmService>();

        private readonly GatewayConfiguration gatewayConfiguration = new GatewayConfiguration
        {
            CmSuffix = "ncg"
        };

        public AuthConfirmControllerTest()
        {
            authConfirmController = new AuthConfirmController(gatewayClient.Object,
                logger.Object,
                gatewayConfiguration, authConfirmService.Object);
        }

        [Fact]
        private void ShouldSendAuthConfirmAndOnAuthConfirm()
        {
            var authConfirmRequest = new AuthConfirmRequest(new Guid().ToString(), "123444");
            AuthConfirmCredential credential = new AuthConfirmCredential(authConfirmRequest.authCode);
            DateTime timeStamp = DateTime.Now.ToUniversalTime();
            var transactionId = TestBuilder.Faker().Random.Hash();
            Guid requestId = Guid.NewGuid();
            var requester = new Requester("1000005", "HIP");
            var validity = new Validity("LINK", requester);
            var onAuthConfirm = new OnConfirmAuth("12", validity, new DateTime(), "1");
            var address = new AuthConfirmAddress(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>());
            var identifiers = new Identifiers("MOBILE", "+919800083232");
            var patient = new AuthConfirmPatient("hina_patel@ncg", "Hina Patel", "F", "1998",
                address, identifiers);
            var onAuthConfirmRequest = new OnAuthConfirmRequest(requestId, timeStamp, onAuthConfirm, patient,
                null, new Resp(requestId.ToString()));
            GatewayAuthConfirmRequestRepresentation gatewayAuthConfirmRequestRepresentation =
                new GatewayAuthConfirmRequestRepresentation(requestId, timeStamp, transactionId, credential);
            var correlationId = Uuid.Generate().ToString();

            authConfirmService.Setup(a => a.AuthConfirmResponse(authConfirmRequest))
                .Returns(gatewayAuthConfirmRequestRepresentation);
            gatewayClient.Setup(
                    client =>
                        client.SendDataToGateway(Constants.PATH_AUTH_CONFIRM,
                            gatewayAuthConfirmRequestRepresentation, "ncg", correlationId))
                .Returns(Task.FromResult(""))
                .Callback<string, GatewayAuthConfirmRequestRepresentation, string, string>
                ((path, gr, cmSuffix, corId)
                    => authConfirmController.OnAuthConfirmRequest(onAuthConfirmRequest));

            if (authConfirmController.AuthConfirmRequest(correlationId, authConfirmRequest).Result is OkObjectResult
                accessToken)
            {
                accessToken.StatusCode.Should().Be(StatusCodes.Status200OK);
                accessToken.Value.Should().BeEquivalentTo("12");
            }
        }

        [Fact]
        private void ShouldNotSendAuthConfirmAndOnAuthConfirm()
        {
            var authConfirmRequest = new AuthConfirmRequest(new Guid().ToString(), "123444");
            AuthConfirmCredential credential = new AuthConfirmCredential(authConfirmRequest.authCode);
            DateTime timeStamp = DateTime.Now.ToUniversalTime();
            var transactionId = TestBuilder.Faker().Random.Hash();
            Guid requestId = Guid.NewGuid();
            GatewayAuthConfirmRequestRepresentation gatewayAuthConfirmRequestRepresentation =
                new GatewayAuthConfirmRequestRepresentation(requestId, timeStamp, transactionId, credential);
            var correlationId = Uuid.Generate().ToString();

            authConfirmService.Setup(a => a.AuthConfirmResponse(authConfirmRequest))
                .Returns(gatewayAuthConfirmRequestRepresentation);
            gatewayClient.Setup(
                    client =>
                        client.SendDataToGateway(Constants.PATH_AUTH_CONFIRM,
                            gatewayAuthConfirmRequestRepresentation, "ncg", correlationId))
                .Returns(Task.FromResult(""));

            try
            {
                authConfirmController.AuthConfirmRequest(correlationId, authConfirmRequest);
            }
            catch (AggregateException ae)
            {
                if (ae.InnerException != null && ae.InnerException.GetType() == typeof(TimeoutException))
                {
                    Assert.True(true);
                    Assert.Contains("Timeout for request_id: " + requestId, ae.Message);
                }
                else
                    Assert.False(false);
            }
        }
    }
}