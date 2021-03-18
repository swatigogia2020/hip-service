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

    public class FetchModeControllerTest
    {
        private readonly FetchModeController _fetchModeController;

        private readonly Mock<ILogger<FetchModeController>> _logger =
            new Mock<ILogger<FetchModeController>>();

        private readonly Mock<GatewayClient> _gatewayClient = new Mock<GatewayClient>(MockBehavior.Strict, null, null);
        private readonly Mock<FetchModeService> _fetchModeService = new Mock<FetchModeService>();

        private readonly GatewayConfiguration _gatewayConfiguration = new GatewayConfiguration
        {
            CmSuffix = "ncg"
        };

        public FetchModeControllerTest()
        {
            _fetchModeController = new FetchModeController(_gatewayClient.Object,
                _logger.Object,
                _gatewayConfiguration, 
                _fetchModeService.Object);
        }

        [Fact]
        private void ShouldFetchPatientsAuthModes()
        {
            var request = new FetchRequest("1234");
            Requester requester = new Requester(_gatewayConfiguration.ClientId, FETCH_MODE_REQUEST_TYPE);
            FetchQuery query = new FetchQuery(request.healthId, FETCH_MODE_PURPOSE, requester);
            DateTime timeStamp = DateTime.Now.ToUniversalTime();
            Guid requestId = Guid.NewGuid();
            LinkageMap.RequestIdToFetchMode.Add(requestId, "12");
            GatewayFetchModesRequestRepresentation gatewayFetchModesRequestRepresentation =
                new GatewayFetchModesRequestRepresentation(requestId, timeStamp, query);
            var correlationId = Uuid.Generate().ToString();

            _fetchModeService.Setup(a => a.FetchModeResponse(request, _gatewayConfiguration))
                .Returns(gatewayFetchModesRequestRepresentation);
            _gatewayClient.Setup(
                    client =>
                        client.SendDataToGateway(Constants.PATH_FETCH_AUTH_MODES,
                            gatewayFetchModesRequestRepresentation, "ncg", correlationId))
                .Returns(Task.FromResult(""));

            if (_fetchModeController.FetchPatientsAuthModes(correlationId, request).Result is OkObjectResult authMode)
            {
                authMode.StatusCode.Should().Be(StatusCodes.Status200OK);
                authMode.Value.Should().BeEquivalentTo("12");
            }
        }
    }
}