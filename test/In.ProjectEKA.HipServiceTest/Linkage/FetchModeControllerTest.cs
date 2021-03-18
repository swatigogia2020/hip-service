using System.Collections.Generic;
using FluentAssertions;
using Hl7.Fhir.Model;
using In.ProjectEKA.HipLibrary.Patient.Model;
using In.ProjectEKA.HipService.Gateway;
using In.ProjectEKA.HipService.Link.Model;
using In.ProjectEKA.HipService.Linkage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Constants = In.ProjectEKA.HipService.Common.Constants;

namespace In.ProjectEKA.HipServiceTest.Linkage
{
    using System;
    using System.Threading.Tasks;
    using Moq;
    using Xunit;
    using static Constants;

    public class FetchModeControllerTest
    {
        private readonly FetchModeController fetchModeController;

        private readonly Mock<ILogger<FetchModeController>> logger =
            new Mock<ILogger<FetchModeController>>();

        private readonly Mock<GatewayClient> gatewayClient = new Mock<GatewayClient>(MockBehavior.Strict, null, null);
        private readonly Mock<FetchModeService> fetchModeService = new Mock<FetchModeService>();

        private readonly GatewayConfiguration gatewayConfiguration = new GatewayConfiguration
        {
            CmSuffix = "ncg"
        };

        public FetchModeControllerTest()
        {
            fetchModeController = new FetchModeController(gatewayClient.Object,
                logger.Object,
                gatewayConfiguration,
                fetchModeService.Object);
        }

        [Fact]
        private void ShouldFetchPatientsAuthModes()
        {
            var request = new FetchRequest("hina_patel@ncg");
            var requester = new Requester(gatewayConfiguration.ClientId, FETCH_MODE_REQUEST_TYPE);
            var query = new FetchQuery(request.healthId, FETCH_MODE_PURPOSE, requester);
            var timeStamp = DateTime.Now.ToUniversalTime();
            var requestId = Guid.NewGuid();
            var gatewayFetchModesRequestRepresentation =
                new GatewayFetchModesRequestRepresentation(requestId, timeStamp, query);
            var correlationId = Uuid.Generate().ToString();
            var modes = new List<Mode>
            {
                {Mode.MOBILE_OTP},
                {Mode.AADHAAR_OTP}
            };
            var onFetchAuthModeRequest = new OnFetchAuthModeRequest(requestId, timeStamp,
                new AuthModeFetch("KYC_AND_LINK", modes), null, new Resp(requestId.ToString()));
            var authModes = string.Join(',', onFetchAuthModeRequest.Auth.Modes);
            fetchModeService.Setup(a => a.FetchModeResponse(request, gatewayConfiguration))
                .Returns(gatewayFetchModesRequestRepresentation);
            gatewayClient.Setup(
                    client =>
                        client.SendDataToGateway(PATH_FETCH_AUTH_MODES,
                            gatewayFetchModesRequestRepresentation, "ncg", correlationId))
                .Returns(Task.FromResult(""))
                .Callback<string, GatewayFetchModesRequestRepresentation, string, string>
                ((path, gr, cmSuffix, corId)
                    => fetchModeController.OnFetchAuthMode(onFetchAuthModeRequest));

            if (fetchModeController.FetchPatientsAuthModes(correlationId, request).Result is OkObjectResult authMode)
            {
                authMode.StatusCode.Should().Be(StatusCodes.Status200OK);
                authMode.Value.Should().BeEquivalentTo(authModes);
            }
        }

        [Fact]
        private void ShouldNotFetchPatientsAuthModes()
        {
            var request = new FetchRequest("hina_patel@ncg");
            var requester = new Requester(gatewayConfiguration.ClientId, FETCH_MODE_REQUEST_TYPE);
            var query = new FetchQuery(request.healthId, FETCH_MODE_PURPOSE, requester);
            var timeStamp = DateTime.Now.ToUniversalTime();
            var requestId = Guid.NewGuid();
            var gatewayFetchModesRequestRepresentation =
                new GatewayFetchModesRequestRepresentation(requestId, timeStamp, query);
            var correlationId = Uuid.Generate().ToString();

            fetchModeService.Setup(a => a.FetchModeResponse(request, gatewayConfiguration))
                .Returns(gatewayFetchModesRequestRepresentation);
            gatewayClient.Setup(
                    client =>
                        client.SendDataToGateway(PATH_FETCH_AUTH_MODES,
                            gatewayFetchModesRequestRepresentation, "ncg", correlationId))
                .Returns(Task.FromResult(""));

            try
            {
                fetchModeController.FetchPatientsAuthModes(correlationId, request);
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