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
    using Moq;
    using Xunit;
    using static Constants;

    public class AuthInitControllerTest
    {
        private readonly AuthInitController _authInitController;

        private readonly Mock<ILogger<AuthInitController>> _logger =
            new Mock<ILogger<AuthInitController>>();


        private readonly Mock<GatewayClient> _gatewayClient = new Mock<GatewayClient>(MockBehavior.Strict, null, null);
        private readonly Mock<AuthInitService> _authInitService = new Mock<AuthInitService>();

        private readonly GatewayConfiguration _gatewayConfiguration = new GatewayConfiguration
        {
            CmSuffix = "ncg"
        };

        public AuthInitControllerTest()
        {
            _authInitController = new AuthInitController(
                _gatewayClient.Object,
                _logger.Object,
                _gatewayConfiguration, 
                _authInitService.Object);
        }

        [Fact]
        private void ShouldSendAuthInitRequest()
        {
            var request = new AuthInitRequest("123", "12344");
            
            DateTime timeStamp = DateTime.Now.ToUniversalTime();
            Requester requester = new Requester(_gatewayConfiguration.ClientId, FETCH_MODE_REQUEST_TYPE);
            AuthInitQuery query = new AuthInitQuery(request.healthId, FETCH_MODE_PURPOSE, request.authMode, requester);
            Guid requestId = Guid.NewGuid();
            LinkageMap.RequestIdToTransactionIdMap.Add(requestId, "12");
            GatewayAuthInitRequestRepresentation gatewayAuthInitRequestRepresentation =
                new GatewayAuthInitRequestRepresentation(requestId, timeStamp, query);
            var correlationId = Uuid.Generate().ToString();
            
            _authInitService.Setup(a => a.AuthInitResponse(request, _gatewayConfiguration))
                .Returns(gatewayAuthInitRequestRepresentation);
            _gatewayClient.Setup(
                client =>
                    client.SendDataToGateway(PATH_AUTH_INIT,
                        gatewayAuthInitRequestRepresentation, "ncg", correlationId))
                .Returns(Task.FromResult(""));
            var transactionId =
                _authInitController.AuthInit(correlationId, request).Result as OkObjectResult;

            if (transactionId != null)
            {
                transactionId.StatusCode.Should().Be(StatusCodes.Status200OK);
                transactionId.Value.Should().BeEquivalentTo("12");
            }
        }
    }
}