using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using In.ProjectEKA.HipService.Common;
using In.ProjectEKA.HipService.Discovery;
using In.ProjectEKA.HipService.Gateway;
using In.ProjectEKA.HipService.Logger;
using In.ProjectEKA.HipService.UserAuth.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace In.ProjectEKA.HipService.UserAuth
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
                logger.LogInformation($"{{cmSuffix}} {{correlationId}} {{authCode}} {{transactionId}} {{requestId}}", cmSuffix, correlationId,
                    authConfirmRequest.authCode, authConfirmRequest.transactionId, requestId);
                await gatewayClient.SendDataToGateway(PATH_AUTH_CONFIRM, gr, cmSuffix, correlationId);
                var i = 0;
                do
                {
                    Thread.Sleep(2000);
                    if (FetchModeMap.requestIdToAccessToken.ContainsKey(requestId))
                    {
                        return FetchModeMap.requestIdToAccessToken[requestId];
                    }
                    
                    i++;
                } while (i < 5);
            }
            catch (Exception exception)
            {
                logger.LogError(LogEvents.Discovery, exception, "Error happened for {RequestId}", requestId);
            }

            return HttpStatusCode.GatewayTimeout.ToString();
        }
        
        [Authorize]
        [HttpPost(ON_AUTH_CONFIRM)]
        public AcceptedResult OnFetchAuthMode(OnAuthConfirmRequest request)
        {
            Log.Information("Auth on confirm request received." +
                            $" RequestId:{request.requestID}, " +
                            $" Timestamp:{request.timestamp}," +
                            $" ResponseRequestId:{request.resp.RequestId}, ");
            if (request.error != null)
            {
                Log.Information($" Error Code:{request.error.Code}," +
                                $" Error Message:{request.error.Message}.");
            }
            else if (request.auth != null)
            {
                string accessToken = request.auth.accessToken;
               
                
                FetchModeMap.requestIdToAccessToken.Add(Guid.Parse(request.resp.RequestId), accessToken);
                
            }

           
            return Accepted();
        }


    }
}