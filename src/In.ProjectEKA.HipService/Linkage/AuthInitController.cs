using System;
using System.Collections.Generic;
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
    
    public class AuthInitController : Controller
    {
        private readonly Dictionary<string, string> requestIdToTransactionIdMap = new Dictionary<string, string>();
        
        private readonly IGatewayClient gatewayClient;
        private readonly IBackgroundJobClient backgroundJob;
        private readonly ILogger<CareContextDiscoveryController> logger;
        private readonly GatewayConfiguration gatewayConfiguration;
        
        public AuthInitController(IGatewayClient gatewayClient, IBackgroundJobClient backgroundJob,
            ILogger<CareContextDiscoveryController> logger, GatewayConfiguration gatewayConfiguration)
        {
            this.gatewayClient = gatewayClient;
            this.backgroundJob = backgroundJob;
            this.logger = logger;
            this.gatewayConfiguration = gatewayConfiguration;
        }
        [Authorize]
        [Route(PATH_HIP_AUTH_INIT)]
        public async Task<string> AuthInit(
            [FromHeader(Name = CORRELATION_ID)] string correlationId, [FromBody] AuthInitRequest authInitRequest)
        {
            string cmSuffix = gatewayConfiguration.CmSuffix;
            Requester requester = new Requester(gatewayConfiguration.ClientId, FETCH_MODE_REQUEST_TYPE);
            AuthInitQuery query = new AuthInitQuery(authInitRequest.healthId, FETCH_MODE_PURPOSE, authInitRequest.authMode, requester);
            DateTime timeStamp = DateTime.Now.ToUniversalTime();
            Guid requestId = Guid.NewGuid();

            GatewayAuthInitRequestRepresentation gatewayAuthInitRequestRepresentation =
                new GatewayAuthInitRequestRepresentation(requestId, timeStamp, query);

            try {
                logger.LogInformation("{cmSuffix} {correlationId}{healthid}", cmSuffix, correlationId,
                    authInitRequest.healthId);
                logger.LogInformation("{gr}",gatewayAuthInitRequestRepresentation.dump(gatewayAuthInitRequestRepresentation));
                
                logger.LogInformation("Calling auth-init of gate way for {requestId} with auth-mode {authMode}",
                    authInitRequest.requestId, authInitRequest.authMode);
                
                await gatewayClient.SendDataToGateway(PATH_AUTH_INIT, gatewayAuthInitRequestRepresentation, cmSuffix, correlationId);
                var i = 0;
                do
                {
                    Thread.Sleep(2000);
                    logger.LogInformation("sleeping");
                    if (requestIdToTransactionIdMap.ContainsKey(requestId.ToString()))
                    {
                        logger.LogInformation(LogEvents.Discovery,
                            "Response about to be send for {RequestId} with {TransactionId}",
                            requestId, requestIdToTransactionIdMap[requestId.ToString()]
                        );
                        return requestIdToTransactionIdMap[requestId.ToString()];
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
        
        [HttpPost(PATH_ON_AUTH_INIT )]
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
                requestIdToTransactionIdMap.Add(request.RequestId, transactionId);
                Log.Information($" For RequestId:{request.RequestId},");
                Log.Information($" TransactionId:{request.Auth.TransactionId}.");
            }
            
            Log.Information($" Resp RequestId:{request.Resp.RequestId}");
            return Accepted();
        }
        
    }
}