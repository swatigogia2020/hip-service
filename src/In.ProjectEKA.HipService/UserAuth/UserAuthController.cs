using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using In.ProjectEKA.HipService.Common;
using In.ProjectEKA.HipService.Discovery;
using In.ProjectEKA.HipService.Gateway;
using In.ProjectEKA.HipService.Link.Model;
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
        private readonly ILogger<CareContextDiscoveryController> logger;
        private readonly GatewayConfiguration gatewayConfiguration;
        private readonly IUserAuthService userAuthService;

        public AuthConfirmController(IGatewayClient gatewayClient,
            ILogger<CareContextDiscoveryController> logger,
            GatewayConfiguration gatewayConfiguration,
            IUserAuthService userAuthService)
        {
            this.gatewayClient = gatewayClient;
            this.logger = logger;
            this.gatewayConfiguration = gatewayConfiguration;
            this.userAuthService = userAuthService;
        }

        [Route(FETCH_MODES)]
        public async Task<string> FetchPatientsAuthModes(
            [FromHeader(Name = CORRELATION_ID)] string correlationId, [FromBody] FetchRequest fetchRequest)
        {
            string cmSuffix = gatewayConfiguration.CmSuffix;
            GatewayFetchModesRequestRepresentation gr =
                userAuthService.FetchModeResponse(fetchRequest, gatewayConfiguration);
            Guid requestId = gr.requestId;

            try
            {
                logger.LogInformation($"{{cmSuffix}} {{correlationId}}{{healthId}} {{requestId}}", cmSuffix,
                    correlationId,
                    fetchRequest.healthId, requestId);
                logger.LogInformation("Request Object: " + gr.dump(gr));

                await gatewayClient.SendDataToGateway(PATH_FETCH_AUTH_MODES, gr, cmSuffix, correlationId);

                var i = 0;
                do
                {
                    Thread.Sleep(2000);
                    if (UserAuthMap.RequestIdToFetchMode.ContainsKey(requestId))
                    {
                        logger.LogInformation(LogEvents.Discovery,
                            "Response about to be send for {RequestId} with {@AuthModes}",
                            requestId, UserAuthMap.RequestIdToFetchMode[requestId]
                        );
                        return UserAuthMap.RequestIdToFetchMode[requestId];
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
        [HttpPost(PATH_ON_FETCH_AUTH_MODES)]
        public AcceptedResult OnFetchAuthMode(OnFetchAuthModeRequest request)
        {
            Log.Information("On fetch mode request received." +
                            $" RequestId:{request.RequestId}, " +
                            $" Timestamp:{request.Timestamp}," +
                            $" ResponseRequestId:{request.Resp.RequestId}, ");
            if (request.Error != null)
            {
                Log.Information($" Error Code:{request.Error.Code}," +
                                $" Error Message:{request.Error.Message}.");
            }
            else if (request.Auth != null)
            {
                string authModes = string.Join(',', request.Auth.Modes);

                UserAuthMap.RequestIdToFetchMode.Add(Guid.Parse(request.Resp.RequestId), authModes);
            }

            Log.Information($" Resp RequestId:{request.Resp.RequestId}");
            return Accepted();
        }

        [Route(PATH_HIP_AUTH_INIT)]
        public async Task<string> AuthInit(
            [FromHeader(Name = CORRELATION_ID)] string correlationId, [FromBody] AuthInitRequest authInitRequest)
        {
            string cmSuffix = gatewayConfiguration.CmSuffix;
            GatewayAuthInitRequestRepresentation gatewayAuthInitRequestRepresentation =
                userAuthService.AuthInitResponse(authInitRequest,gatewayConfiguration);
            Guid requestId = gatewayAuthInitRequestRepresentation.requestId;

            try
            {
                await gatewayClient.SendDataToGateway(PATH_AUTH_INIT, gatewayAuthInitRequestRepresentation, cmSuffix,
                    correlationId);
                var i = 0;
                do
                {
                    Thread.Sleep(2000);
                    if (UserAuthMap.RequestIdToTransactionIdMap.ContainsKey(requestId))
                    {
                        logger.LogInformation(LogEvents.Discovery,
                            "Response about to be send for {RequestId} with {TransactionId}",
                            requestId, UserAuthMap.RequestIdToTransactionIdMap[requestId]
                        );
                        return UserAuthMap.RequestIdToTransactionIdMap[requestId];
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
                UserAuthMap.RequestIdToTransactionIdMap.Add(Guid.Parse(request.Resp.RequestId), transactionId);
            }

            return Accepted();
        }

        [Route(HIP_AUTH_CONFIRM)]
        public async Task<string> FetchPatientsAuthModes(
            [FromHeader(Name = CORRELATION_ID)] string correlationId, [FromBody] AuthConfirmRequest authConfirmRequest)
        {
            string cmSuffix = gatewayConfiguration.CmSuffix;
            GatewayAuthConfirmRequestRepresentation gatewayAuthConfirmRequestRepresentation =
                userAuthService.AuthConfirmResponse(authConfirmRequest);
            Guid requestId = gatewayAuthConfirmRequestRepresentation.requestId;

            try
            {
                logger.LogInformation($"{{cmSuffix}} {{correlationId}} {{authCode}} {{transactionId}} {{requestId}}",
                    cmSuffix, correlationId,
                    authConfirmRequest.authCode, authConfirmRequest.transactionId, requestId);
                await gatewayClient.SendDataToGateway(PATH_AUTH_CONFIRM, gatewayAuthConfirmRequestRepresentation
                    , cmSuffix, correlationId);
                var i = 0;
                do
                {
                    Thread.Sleep(2000);
                    if (UserAuthMap.RequestIdToAccessToken.ContainsKey(requestId))
                    {
                        return UserAuthMap.RequestIdToAccessToken[requestId];
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


                UserAuthMap.RequestIdToAccessToken.Add(Guid.Parse(request.resp.RequestId), accessToken);
            }

            return Accepted();
        }
    }
}