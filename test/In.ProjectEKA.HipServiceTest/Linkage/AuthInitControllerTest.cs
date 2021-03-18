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
        private readonly AuthInitController authInitController;

        private readonly Mock<ILogger<AuthInitController>> logger =
            new Mock<ILogger<AuthInitController>>();


        private readonly Mock<GatewayClient> gatewayClient = new Mock<GatewayClient>(MockBehavior.Strict, null, null);
        private readonly Mock<AuthInitService> authInitService = new Mock<AuthInitService>();

        private readonly GatewayConfiguration gatewayConfiguration = new GatewayConfiguration
        {
            CmSuffix = "ncg"
        };

        public AuthInitControllerTest()
        {
            authInitController = new AuthInitController(
                gatewayClient.Object,
                logger.Object,
                gatewayConfiguration, 
                authInitService.Object);
        }

        [Fact]
        private void ShouldSendAuthInitRequest()
        {
            var request = new AuthInitRequest("123", "12344");
            
            DateTime timeStamp = DateTime.Now.ToUniversalTime();
            Requester requester = new Requester(gatewayConfiguration.ClientId, FETCH_MODE_REQUEST_TYPE);
            AuthInitQuery query = new AuthInitQuery(request.healthId, FETCH_MODE_PURPOSE, request.authMode, requester);
            Guid requestId = Guid.NewGuid();
            LinkageMap.RequestIdToTransactionIdMap.Add(requestId, "12");
            GatewayAuthInitRequestRepresentation gatewayAuthInitRequestRepresentation =
                new GatewayAuthInitRequestRepresentation(requestId, timeStamp, query);
            var correlationId = Uuid.Generate().ToString();
            
            authInitService.Setup(a => a.AuthInitResponse(request, gatewayConfiguration))
                .Returns(gatewayAuthInitRequestRepresentation);
            gatewayClient.Setup(
                client =>
                    client.SendDataToGateway(PATH_AUTH_INIT,
                        gatewayAuthInitRequestRepresentation, "ncg", correlationId))
                .Returns(Task.FromResult(""));

            if (authInitController.AuthInit(correlationId, request).Result is OkObjectResult transactionId)
            {
                transactionId.StatusCode.Should().Be(StatusCodes.Status200OK);
                transactionId.Value.Should().BeEquivalentTo("12");
            }
        }
    }
}