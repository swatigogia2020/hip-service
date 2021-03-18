using System;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly IAuthConfirmService authConfirmService;

        public AuthConfirmController(GatewayClient gatewayClient,
            ILogger<AuthConfirmController> logger, GatewayConfiguration gatewayConfiguration,
            IAuthConfirmService authConfirmService)
        {
            this.gatewayClient = gatewayClient;
            this.logger = logger;
            this.gatewayConfiguration = gatewayConfiguration;
            this.authConfirmService = authConfirmService;
        }

        [Route(HIP_AUTH_CONFIRM)]
        public async Task<ActionResult> AuthConfirmRequest(
            [FromHeader(Name = CORRELATION_ID)] string correlationId, [FromBody] AuthConfirmRequest authConfirmRequest)
        {
            string cmSuffix = gatewayConfiguration.CmSuffix;
            GatewayAuthConfirmRequestRepresentation gatewayAuthConfirmRequestRepresentation =
                authConfirmService.AuthConfirmResponse(authConfirmRequest);
            var requestId = gatewayAuthConfirmRequestRepresentation.requestId.ToString();
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
                    if (LinkageMap.RequestIdToAccessToken.ContainsKey(requestId))
                    {
                        logger.LogInformation(LogEvents.AuthConfirm,
                            "Response about to be send for {@RequestId} with {@AccessToken}",
                            requestId, LinkageMap.RequestIdToAccessToken[requestId]
                        );
                        return Ok(LinkageMap.RequestIdToAccessToken[requestId]);
                    }

                    i++;
                } while (i < 5);
            }
            catch (Exception exception)
            {
                logger.LogError(LogEvents.AuthConfirm, exception, "Error happened for {RequestId}", requestId);
            }

            throw new TimeoutException("Timeout for request_id: " + requestId);
        }

        [Authorize]
        [HttpPost(ON_AUTH_CONFIRM)]
        public AcceptedResult OnAuthConfirmRequest(OnAuthConfirmRequest request)
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
                LinkageMap.RequestIdToAccessToken.Add(request.resp.RequestId, accessToken);
                Log.Information($" requestID:{request.requestID},");
                Log.Information($" accessToken:{accessToken}.");
            }

            Log.Information($" Resp RequestId:{request.resp.RequestId}");
            return Accepted();
        }
    }
}