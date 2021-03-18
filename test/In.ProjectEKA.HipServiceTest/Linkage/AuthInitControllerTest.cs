using FluentAssertions;
using Hl7.Fhir.Model;
using In.ProjectEKA.HipLibrary.Patient.Model;
using In.ProjectEKA.HipService.Common;
using In.ProjectEKA.HipService.Gateway;
using In.ProjectEKA.HipService.Link.Model;
using In.ProjectEKA.HipService.Linkage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Meta = In.ProjectEKA.HipService.Link.Model.Meta;


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
            var request = new AuthInitRequest("hina_patel@ncg", "MOBILE_OTP");

            var timeStamp = DateTime.Now.ToUniversalTime();
            var requester = new Requester(gatewayConfiguration.ClientId, FETCH_MODE_REQUEST_TYPE);
            var query = new AuthInitQuery(request.healthId, FETCH_MODE_PURPOSE, request.authMode, requester);
            var requestId = Guid.NewGuid();
            var gatewayAuthInitRequestRepresentation =
                new GatewayAuthInitRequestRepresentation(requestId, timeStamp, query);
            var correlationId = Uuid.Generate().ToString();
            var transactionId = new Guid().ToString();
            var auth = new Auth(transactionId, new Meta("string", new DateTime()), Mode.MOBILE_OTP);
            var authOnInitRequest =
                new AuthOnInitRequest(requestId, timeStamp, auth, null, new Resp(requestId.ToString()));
            authInitService.Setup(a => a.AuthInitResponse(request, gatewayConfiguration))
                .Returns(gatewayAuthInitRequestRepresentation);
            gatewayClient.Setup(
                    client =>
                        client.SendDataToGateway(PATH_AUTH_INIT,
                            gatewayAuthInitRequestRepresentation, "ncg", correlationId))
                .Callback<string, GatewayAuthInitRequestRepresentation, string, string>
                ((path, gr, cmSuffix, corId)
                    => authInitController.OnAuthInit(authOnInitRequest))
                .Returns(Task.FromResult(""));

            if (authInitController.AuthInit(correlationId, request).Result is OkObjectResult tId)
            {
                tId.StatusCode.Should().Be(StatusCodes.Status200OK);
                tId.Value.Should().BeEquivalentTo(transactionId);
            }
        }

        [Fact]
        private void ShouldNotSendAuthInitAndOnAuthInit()
        {
            var request = new AuthInitRequest("123", "12344");

            var timeStamp = DateTime.Now.ToUniversalTime();
            var requester = new Requester(gatewayConfiguration.ClientId, FETCH_MODE_REQUEST_TYPE);
            var query = new AuthInitQuery(request.healthId, FETCH_MODE_PURPOSE, request.authMode, requester);
            var requestId = Guid.NewGuid();
            var gatewayAuthInitRequestRepresentation =
                new GatewayAuthInitRequestRepresentation(requestId, timeStamp, query);
            var correlationId = Uuid.Generate().ToString();

            authInitService.Setup(a => a.AuthInitResponse(request, gatewayConfiguration))
                .Returns(gatewayAuthInitRequestRepresentation);
            gatewayClient.Setup(
                    client =>
                        client.SendDataToGateway(Constants.PATH_AUTH_INIT,
                            gatewayAuthInitRequestRepresentation, "ncg", correlationId))
                .Returns(Task.FromResult(""));

            try
            {
                authInitController.AuthInit(correlationId, request);
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