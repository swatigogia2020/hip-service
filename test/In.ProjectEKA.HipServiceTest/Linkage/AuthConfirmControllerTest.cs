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
        private readonly Mock<IBackgroundJobClient> _backgroundJobClient = new Mock<IBackgroundJobClient>();
        private readonly AuthConfirmController _authConfirmController;

        private readonly Mock<ILogger<AuthConfirmController>> _logger =
            new Mock<ILogger<AuthConfirmController>>();

        private readonly Mock<GatewayClient> _gatewayClient = new Mock<GatewayClient>(MockBehavior.Strict, null, null);
        private readonly Mock<AuthConfirmService> _authConfirmService = new Mock<AuthConfirmService>();

        private readonly GatewayConfiguration _gatewayConfiguration = new GatewayConfiguration
        {
            CmSuffix = "ncg"
        };

        public AuthConfirmControllerTest()
        {
            _authConfirmController = new AuthConfirmController(_gatewayClient.Object,
                _logger.Object,
                _gatewayConfiguration, _authConfirmService.Object);
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

            _authConfirmService.Setup(a => a.authConfirmResponse(request))
                .Returns(gatewayAuthConfirmRequestRepresentation);
            _gatewayClient.Setup(
                    client =>
                        client.SendDataToGateway(Constants.PATH_AUTH_CONFIRM,
                            gatewayAuthConfirmRequestRepresentation, "ncg", correlationId))
                .Returns(Task.FromResult(""));
            var accessToken =
                _authConfirmController.FetchPatientsAuthModes(correlationId, request).Result as OkObjectResult;

            if (accessToken != null)
            {
                accessToken.StatusCode.Should().Be(StatusCodes.Status200OK);
                accessToken.Value.Should().BeEquivalentTo("12");
            }
        }
    }
}