using FluentAssertions;
using Hl7.Fhir.Model;
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
    using Hangfire;
    using Moq;
    using Xunit;

    public class AuthConfirmControllerTest
    {
        private readonly Mock<IBackgroundJobClient> backgroundJobClient = new Mock<IBackgroundJobClient>();
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
                backgroundJobClient.Object,
                logger.Object,
                gatewayConfiguration, authConfirmService.Object);
        }

        [Fact]
        private void ShouldSendAuthConfirm()
        {
            var request = new AuthConfirmRequest("123", "12344");
            AuthConfirmCredential credential = new AuthConfirmCredential(request.authCode);
            DateTime timeStamp = DateTime.Now.ToUniversalTime();
            var transactionId = TestBuilder.Faker().Random.Hash();
            Guid requestId = Guid.NewGuid();
            FetchModeMap.RequestIdToAccessToken.Add(requestId, "12");
            GatewayAuthConfirmRequestRepresentation gatewayAuthConfirmRequestRepresentation =
                new GatewayAuthConfirmRequestRepresentation(requestId, timeStamp, transactionId, credential);
            var correlationId = Uuid.Generate().ToString();
            
            authConfirmService.Setup(a => a.authConfirmResponse(request))
                .Returns(gatewayAuthConfirmRequestRepresentation);
            gatewayClient.Setup(
                client =>
                    client.SendDataToGateway(Constants.PATH_AUTH_CONFIRM,
                        gatewayAuthConfirmRequestRepresentation, "ncg", correlationId))
                .Returns(Task.FromResult(""));
            var accessToken =
                authConfirmController.FetchPatientsAuthModes(correlationId, request).Result as OkObjectResult;
            
            accessToken.StatusCode.Should().Be(StatusCodes.Status200OK);
            accessToken.Value.Should().BeEquivalentTo("12");
        }
    }
}