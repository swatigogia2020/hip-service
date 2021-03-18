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
    public class AuthInitController : Controller
    {
        private readonly IGatewayClient gatewayClient;
        private readonly ILogger<AuthInitController> logger;
        private readonly GatewayConfiguration gatewayConfiguration;
        private readonly IAuthInitService authInitService;

        public AuthInitController(IGatewayClient gatewayClient,
            ILogger<AuthInitController> logger, GatewayConfiguration gatewayConfiguration,
            IAuthInitService authInitService)
        {
            this.gatewayClient = gatewayClient;
            this.logger = logger;
            this.gatewayConfiguration = gatewayConfiguration;
            this.authInitService = authInitService;
        }

        [Route(PATH_HIP_AUTH_INIT)]
        public async Task<ActionResult> AuthInit(
            [FromHeader(Name = CORRELATION_ID)] string correlationId, [FromBody] AuthInitRequest authInitRequest)
        {
            string cmSuffix = gatewayConfiguration.CmSuffix;
            GatewayAuthInitRequestRepresentation gatewayAuthInitRequestRepresentation =
                authInitService.AuthInitResponse(authInitRequest, gatewayConfiguration);
            var requestId = gatewayAuthInitRequestRepresentation.requestId.ToString();

            try
            {
                logger.Log(LogLevel.Error,
                    LogEvents.AuthInit,
                    "Request for auth-init to gateway: {@GatewayResponse}",
                    gatewayAuthInitRequestRepresentation);
                await gatewayClient.SendDataToGateway(PATH_AUTH_INIT, gatewayAuthInitRequestRepresentation, cmSuffix,
                    correlationId);
                var i = 0;
                do
                {
                    Thread.Sleep(2000);
                    if (LinkageMap.RequestIdToTransactionIdMap.ContainsKey(requestId))
                    {
                        logger.LogInformation(LogEvents.Discovery,
                            "Response about to be send for {RequestId} with {TransactionId}",
                            requestId, LinkageMap.RequestIdToTransactionIdMap[requestId]
                        );
                        return Ok(LinkageMap.RequestIdToTransactionIdMap[requestId]);
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
        [HttpPost(PATH_ON_AUTH_INIT)]
        public AcceptedResult OnAuthInit(AuthOnInitRequest request)
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
                string transactionId = request.Auth.TransactionId;
                LinkageMap.RequestIdToTransactionIdMap.Add(request.Resp.RequestId, transactionId);
            }

            return Accepted();
        }
    }
}