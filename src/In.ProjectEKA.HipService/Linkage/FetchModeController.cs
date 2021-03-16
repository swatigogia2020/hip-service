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
    public class FetchModeController : Controller
    {
        private readonly IGatewayClient gatewayClient;

        private readonly IBackgroundJobClient backgroundJob;
        private readonly ILogger<CareContextDiscoveryController> logger;
        private readonly GatewayConfiguration gatewayConfiguration;

        public FetchModeController(IGatewayClient gatewayClient, IBackgroundJobClient backgroundJob,
            ILogger<CareContextDiscoveryController> logger, GatewayConfiguration gatewayConfiguration)
        {
            this.gatewayClient = gatewayClient;
            this.backgroundJob = backgroundJob;
            this.logger = logger;
            this.gatewayConfiguration = gatewayConfiguration;
        }
        
        [Route(FETCH_MODES)]
        public async Task<string> FetchPatientsAuthModes(
            [FromHeader(Name = CORRELATION_ID)] string correlationId, [FromBody] FetchRequest fetchRequest)
        {
            string cmSuffix = gatewayConfiguration.CmSuffix;
            Requester requester = new Requester(gatewayConfiguration.ClientId, FETCH_MODE_REQUEST_TYPE);
            FetchQuery query = new FetchQuery(fetchRequest.healthId, FETCH_MODE_PURPOSE, requester);
            DateTime timeStamp = DateTime.Now.ToUniversalTime();
            Guid requestId = Guid.NewGuid();
            GatewayFetchModesRequestRepresentation gr =
                new GatewayFetchModesRequestRepresentation(requestId, timeStamp, query);
            
            try
            {
                logger.LogInformation("{cmSuffix} {correlationId}{healthid}", cmSuffix, correlationId,
                    fetchRequest.healthId);
                
                await gatewayClient.SendDataToGateway(PATH_FETCH_AUTH_MODES, gr, cmSuffix, correlationId);
                
                var i = 0;
                do
                {
                    Thread.Sleep(2000);
                    if (FetchModeMap.requestIdToFetchMode.ContainsKey(requestId))
                    {
                        logger.LogInformation(LogEvents.Discovery,
                            "Response about to be send for {RequestId} with {@AuthModes}",
                            requestId, FetchModeMap.requestIdToFetchMode[requestId]
                        );
                        return FetchModeMap.requestIdToFetchMode[requestId];
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

                string authModes =string.Join(',', request.Auth.Modes);
                
                FetchModeMap.requestIdToFetchMode.Add(request.RequestId, authModes);
            }

            Log.Information($" Resp RequestId:{request.Resp.RequestId}");
            return Accepted();
        }
    }
}