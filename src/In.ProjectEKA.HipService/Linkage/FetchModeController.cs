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
        private readonly IGatewayClient gatewayClient;
        private readonly ILogger<FetchModeController> logger;
        private readonly GatewayConfiguration gatewayConfiguration;
        private readonly IFetchModeService fetchModeService;

        public FetchModeController(IGatewayClient gatewayClient,
            ILogger<FetchModeController> logger, GatewayConfiguration gatewayConfiguration,
            IFetchModeService fetchModeService)
        {
            this.gatewayClient = gatewayClient;
            this.logger = logger;
            this.gatewayConfiguration = gatewayConfiguration;
            this.fetchModeService = fetchModeService;
        }

        [Route(FETCH_MODES)]
        public async Task<ActionResult> FetchPatientsAuthModes(
            [FromHeader(Name = CORRELATION_ID)] string correlationId, [FromBody] FetchRequest fetchRequest)
        {
            string cmSuffix = gatewayConfiguration.CmSuffix;
            GatewayFetchModesRequestRepresentation gr =
                fetchModeService.FetchModeResponse(fetchRequest, gatewayConfiguration);
            var requestId = gr.requestId.ToString();
            try
            {
                logger.Log(LogLevel.Error,
                    LogEvents.FetchMode,
                    "Request for fetch-modee to gateway: {@GatewayResponse}",
                    gr);
                await gatewayClient.SendDataToGateway(PATH_FETCH_AUTH_MODES, gr, cmSuffix, correlationId);
                var i = 0;
                do
                {
                    Thread.Sleep(2000);
                    if (LinkageMap.RequestIdToFetchMode.ContainsKey(requestId))
                    {
                        logger.LogInformation(LogEvents.Discovery,
                            "Response about to be send for {RequestId} with {@AuthModes}",
                            requestId, LinkageMap.RequestIdToFetchMode[requestId]
                        );
                        return Ok(LinkageMap.RequestIdToFetchMode[requestId]);
                    }

                    i++;
                } while (i < 5);
            }
            catch (Exception exception)
            {
                logger.LogError(LogEvents.Discovery, exception, "Error happened for {RequestId}", requestId);
            }

            throw new TimeoutException("Timeout for request_id: " + requestId);
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

                LinkageMap.RequestIdToFetchMode.Add(request.Resp.RequestId, authModes);
            }

            Log.Information($" Resp RequestId:{request.Resp.RequestId}");
            return Accepted();
        }
    }
}