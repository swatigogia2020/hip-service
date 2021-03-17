using System;
using System.Threading;
using System.Threading.Tasks;
using In.ProjectEKA.HipService.Common;
using In.ProjectEKA.HipService.Gateway;
using In.ProjectEKA.HipService.Link.Model;
using In.ProjectEKA.HipService.Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace In.ProjectEKA.HipService.Linkage
{
    using static Constants;

    [ApiController]
    public class FetchModeController : Controller
    {
        private readonly IGatewayClient _gatewayClient;
        private readonly ILogger<FetchModeController> _logger;
        private readonly GatewayConfiguration _gatewayConfiguration;
        private readonly IFetchModeService _fetchModeService;

        public FetchModeController(IGatewayClient gatewayClient,
            ILogger<FetchModeController> logger, GatewayConfiguration gatewayConfiguration,
            IFetchModeService fetchModeService)
        {
            _gatewayClient = gatewayClient;
            _logger = logger;
            _gatewayConfiguration = gatewayConfiguration;
            _fetchModeService = fetchModeService;
        }

        [Route(FETCH_MODES)]
        public async Task<ActionResult> FetchPatientsAuthModes(
            [FromHeader(Name = CORRELATION_ID)] string correlationId, [FromBody] FetchRequest fetchRequest)
        {
            string cmSuffix = _gatewayConfiguration.CmSuffix;
            GatewayFetchModesRequestRepresentation gr =
                _fetchModeService.FetchModeResponse(fetchRequest, _gatewayConfiguration);

            try
            {
                _logger.LogInformation("{cmSuffix} {correlationId}{healthid}", cmSuffix, correlationId,
                    fetchRequest.healthId);

                await _gatewayClient.SendDataToGateway(PATH_FETCH_AUTH_MODES, gr, cmSuffix, correlationId);

                var i = 0;
                do
                {
                    Thread.Sleep(2000);
                    if (FetchModeMap.requestIdToFetchMode.ContainsKey(gr.requestId))
                    {
                        _logger.LogInformation(LogEvents.Discovery,
                            "Response about to be send for {RequestId} with {@AuthModes}",
                            gr.requestId, FetchModeMap.requestIdToFetchMode[gr.requestId]
                        );
                        return Ok(FetchModeMap.requestIdToFetchMode[gr.requestId]);
                    }

                    i++;
                } while (i < 5);
            }
            catch (Exception exception)
            {
                _logger.LogError(LogEvents.Discovery, exception, "Error happened for {RequestId}", gr.requestId);
            }

            throw new TimeoutException("Timeout for request_id: " + gr.requestId);
        }

        [Authorize]
        [HttpPost(PATH_ON_FETCH_AUTH_MODES)]
        public AcceptedResult OnFetchAuthMode(OnFetchAuthModeRequest request)
        {
            Log.Information("Auth on init request received." +
                            $" RequestId:{request.RequestId}, " +
                            $" Timestamp:{request.Timestamp},");
            if (request.Error != null)
            {
                Log.Information($" Error Code:{request.Error.Code}," +
                                $" Error Message:{request.Error.Message}.");
            }
            else if (request.Auth != null)
            {
                string authModes = string.Join(',', request.Auth.Modes);

                FetchModeMap.requestIdToFetchMode.Add(request.RequestId, authModes);
            }

            Log.Information($" Resp RequestId:{request.Resp.RequestId}");
            return Accepted();
        }
    }
}