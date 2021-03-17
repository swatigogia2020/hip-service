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
        private readonly IGatewayClient _gatewayClient;
        private readonly ILogger<AuthInitController> _logger;
        private readonly GatewayConfiguration _gatewayConfiguration;
        private readonly IAuthInitService _authInitService;

        public AuthInitController(IGatewayClient gatewayClient,
            ILogger<AuthInitController> logger, GatewayConfiguration gatewayConfiguration,
            IAuthInitService authInitService)
        {
            _gatewayClient = gatewayClient;
            _logger = logger;
            _gatewayConfiguration = gatewayConfiguration;
            _authInitService = authInitService;
        }

        [Route(PATH_HIP_AUTH_INIT)]
        public async Task<ActionResult> AuthInit(
            [FromHeader(Name = CORRELATION_ID)] string correlationId, [FromBody] AuthInitRequest authInitRequest)
        {
            string cmSuffix = _gatewayConfiguration.CmSuffix;
            GatewayAuthInitRequestRepresentation gatewayAuthInitRequestRepresentation =
                _authInitService.AuthInitResponse(authInitRequest, _gatewayConfiguration);
            var requestId = gatewayAuthInitRequestRepresentation.requestId;

            try
            {
                await _gatewayClient.SendDataToGateway(PATH_AUTH_INIT, gatewayAuthInitRequestRepresentation, cmSuffix,
                    correlationId);
                var i = 0;
                do
                {
                    Thread.Sleep(2000);
                    if (FetchModeMap.requestIdToTransactionIdMap.ContainsKey(requestId))
                    {
                        _logger.LogInformation(LogEvents.Discovery,
                            "Response about to be send for {RequestId} with {TransactionId}",
                            requestId, FetchModeMap.requestIdToTransactionIdMap[requestId]
                        );
                        return Ok(FetchModeMap.requestIdToTransactionIdMap[requestId]);
                    }

                    i++;
                } while (i < 5);
            }
            catch (Exception exception)
            {
                _logger.LogError(LogEvents.Discovery, exception, "Error happened for {RequestId}", requestId);
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
                FetchModeMap.requestIdToTransactionIdMap.Add(request.RequestId, transactionId);
            }

            return Accepted();
        }
    }
}