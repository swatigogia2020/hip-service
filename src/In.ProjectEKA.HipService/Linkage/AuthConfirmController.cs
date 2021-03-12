using System;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using In.ProjectEKA.HipService.Common;
using In.ProjectEKA.HipService.Discovery;
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
   
    public class AuthConfirmController : Controller
    {
        private readonly IGatewayClient gatewayClient;

        private readonly IBackgroundJobClient backgroundJob;
        private readonly ILogger<CareContextDiscoveryController> logger;
        private readonly GatewayConfiguration gatewayConfiguration;

        public AuthConfirmController(IGatewayClient gatewayClient, IBackgroundJobClient backgroundJob,
            ILogger<CareContextDiscoveryController> logger, GatewayConfiguration gatewayConfiguration)
        {
            this.gatewayClient = gatewayClient;
            this.backgroundJob = backgroundJob;
            this.logger = logger;
            this.gatewayConfiguration = gatewayConfiguration;
        }

        [Route(HIP_AUTH_CONFIRM)]
        public async Task<string> FetchPatientsAuthModes(
            [FromHeader(Name = CORRELATION_ID)] string correlationId, [FromBody] AuthConfirmRequest authConfirmRequest)
        {
            string cmSuffix = gatewayConfiguration.CmSuffix;
            AuthConfirmCredential credential = new AuthConfirmCredential(authConfirmRequest.authCode);
            string transactionId = authConfirmRequest.transactionId;
            DateTime timeStamp = DateTime.Now.ToUniversalTime();
            Guid requestId = Guid.NewGuid();
            GatewayAuthConfirmRequestRepresentation gr =
                new GatewayAuthConfirmRequestRepresentation(requestId, timeStamp, transactionId, credential);
            
            try
            {
                logger.LogInformation("{cmSuffix} {correlationId} {authCode} {transactionId}", cmSuffix, correlationId,
                    authConfirmRequest.authCode, authConfirmRequest.transactionId);
                logger.LogInformation("{gr}",gr.dump(gr));
                await gatewayClient.SendDataToGateway(PATH_AUTH_CONFIRM, gr, cmSuffix, correlationId);
                //requestmap.add(reqId, [""]); return if the reqid ia lready in the map
                var i = 0;
                do
                {
                    Thread.Sleep(2000);
                    logger.LogInformation("sleeping");
                    if (FetchModeMap.requestIdToAccessToken.ContainsKey(requestId))
                    {
                        logger.LogInformation(LogEvents.Discovery,
                            "Response about to be send for {RequestId} with {@AuthModes}",
                            requestId, FetchModeMap.requestIdToAccessToken[requestId]
                        );
                        return FetchModeMap.requestIdToAccessToken[requestId];
                    }
                    
                    i++;
                } while (i < 5);
            }
            catch (Exception exception)
            {
                logger.LogError(LogEvents.Discovery, exception, "Error happened for {RequestId}", requestId);
            }

            return "";
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
               
                
                FetchModeMap.requestIdToAccessToken.Add(request.requestID, accessToken);
                
                Log.Information($" requestID:{request.requestID},");
                Log.Information($" accessToken:{accessToken}.");
            }

            Log.Information($" Resp RequestId:{request.resp.RequestId}");
            return Accepted();
        }


    }
}