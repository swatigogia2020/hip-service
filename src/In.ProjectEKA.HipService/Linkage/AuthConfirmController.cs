using System;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using In.ProjectEKA.HipService.Common;
using In.ProjectEKA.HipService.Gateway;
using In.ProjectEKA.HipService.Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace In.ProjectEKA.HipService.Linkage
{
    using static Constants;

    [ApiController]
    public class AuthConfirmController : Controller
    {
        private readonly GatewayClient gatewayClient;
        private readonly ILogger<AuthConfirmController> logger;
        private readonly GatewayConfiguration gatewayConfiguration;
        private readonly AuthConfirmService authConfirmService;

        public AuthConfirmController(GatewayClient gatewayClient, IBackgroundJobClient backgroundJob,
            ILogger<AuthConfirmController> logger, GatewayConfiguration gatewayConfiguration,
            AuthConfirmService authConfirmService)
        {
            this.gatewayClient = gatewayClient;
            this.logger = logger;
            this.gatewayConfiguration = gatewayConfiguration;
            this.authConfirmService = authConfirmService;
        }

        [Route(HIP_AUTH_CONFIRM)]
        public async Task<ActionResult> FetchPatientsAuthModes(
            [FromHeader(Name = CORRELATION_ID)] string correlationId, [FromBody] AuthConfirmRequest authConfirmRequest)
        {
            string cmSuffix = gatewayConfiguration.CmSuffix;
            GatewayAuthConfirmRequestRepresentation gatewayAuthConfirmRequestRepresentation =
                authConfirmService.authConfirmResponse(authConfirmRequest);
            var requestId = gatewayAuthConfirmRequestRepresentation.requestId;
            try
            {
                logger.Log(LogLevel.Error,
                    LogEvents.AuthConfirm,
                    "Request for auth-confirm to gateway: {@GatewayResponse}",
                    gatewayAuthConfirmRequestRepresentation);
                await gatewayClient.SendDataToGateway(PATH_AUTH_CONFIRM, gatewayAuthConfirmRequestRepresentation,
                    cmSuffix, correlationId);
                var i = 0;
                do
                {
                    Thread.Sleep(2000);
                    logger.LogInformation("sleeping");
                    if (FetchModeMap.RequestIdToAccessToken.ContainsKey(requestId))
                    {
                        logger.LogInformation(LogEvents.Discovery,
                            "Response about to be send for {@RequestId} with {@AccessToken}",
                            requestId, FetchModeMap.RequestIdToAccessToken[requestId]
                        );
                        return Ok(FetchModeMap.RequestIdToAccessToken[requestId]);
                    }

                    i++;
                } while (i < 5);
            }
            catch (Exception exception)
            {
                logger.LogError(LogEvents.Discovery, exception, "Error happened for {RequestId}", requestId);
            }

            return Ok("");
        }

        [Authorize]
        [HttpPost(ON_AUTH_CONFIRM)]
        public AcceptedResult OnFetchAuthMode(OnAuthConfirmRequest request)
        {
            Log.Information("Auth on init request received." +
                            $" RequestId:{request.requestID}, " +
                            $" Timestamp:{request.timestamp},");
            if (request.error != null)
            {
                Log.Information($" Error Code:{request.error.Code}," +
                                $" Error Message:{request.error.Message}.");
            }
            else if (request.auth != null)
            {
                string accessToken = request.auth.accessToken;
                FetchModeMap.RequestIdToAccessToken.Add(request.requestID, accessToken);
                Log.Information($" requestID:{request.requestID},");
                Log.Information($" accessToken:{accessToken}.");
            }

            Log.Information($" Resp RequestId:{request.resp.RequestId}");
            return Accepted();
        }
    }
}