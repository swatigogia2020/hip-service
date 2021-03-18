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

        public AuthConfirmController(IGatewayClient gatewayClient,
            ILogger<CareContextDiscoveryController> logger,
            GatewayConfiguration gatewayConfiguration)
        {
            this.gatewayClient = gatewayClient;
            this.logger = logger;
            this.gatewayConfiguration = gatewayConfiguration;
        }

        [Route(FETCH_MODES)]
        public async Task<string> FetchPatientsAuthModes(
            [FromHeader(Name = CORRELATION_ID)] string correlationId, [FromBody] FetchRequest fetchRequest)
        {
            string cmSuffix = gatewayConfiguration.CmSuffix;
            Requester requester = new Requester("Bahmni", FETCH_MODE_REQUEST_TYPE);
            FetchQuery query = new FetchQuery(fetchRequest.healthId, FETCH_MODE_PURPOSE, requester);
            DateTime timeStamp = DateTime.Now.ToUniversalTime();
            Guid requestId = Guid.NewGuid();
            GatewayFetchModesRequestRepresentation gr =
                new GatewayFetchModesRequestRepresentation(requestId, timeStamp, query);

            try
            {
                logger.LogInformation($"{{cmSuffix}} {{correlationId}}{{healthid}} {{requestId}}", cmSuffix,
                    correlationId,
                    fetchRequest.healthId, requestId);
                logger.LogInformation("Request Object: " + gr.dump(gr));

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

                FetchModeMap.requestIdToFetchMode.Add(Guid.Parse(request.Resp.RequestId), authModes);
            }

            Log.Information($" Resp RequestId:{request.Resp.RequestId}");
            return Accepted();
        }

        [Route(PATH_HIP_AUTH_INIT)]
        public async Task<string> AuthInit(
            [FromHeader(Name = CORRELATION_ID)] string correlationId, [FromBody] AuthInitRequest authInitRequest)
        {
            string cmSuffix = gatewayConfiguration.CmSuffix;
            Requester requester = new Requester("Bahmni", FETCH_MODE_REQUEST_TYPE);
            AuthInitQuery query = new AuthInitQuery(authInitRequest.healthId, FETCH_MODE_PURPOSE,
                authInitRequest.authMode, requester);
            DateTime timeStamp = DateTime.Now.ToUniversalTime();
            Guid requestId = Guid.NewGuid();

            GatewayAuthInitRequestRepresentation gatewayAuthInitRequestRepresentation =
                new GatewayAuthInitRequestRepresentation(requestId, timeStamp, query);

            try
            {
                await gatewayClient.SendDataToGateway(PATH_AUTH_INIT, gatewayAuthInitRequestRepresentation, cmSuffix,
                    correlationId);
                var i = 0;
                do
                {
                    Thread.Sleep(2000);
                    if (FetchModeMap.requestIdToTransactionIdMap.ContainsKey(requestId))
                    {
                        logger.LogInformation(LogEvents.Discovery,
                            "Response about to be send for {RequestId} with {TransactionId}",
                            requestId, FetchModeMap.requestIdToTransactionIdMap[requestId]
                        );
                        return FetchModeMap.requestIdToTransactionIdMap[requestId];
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
                FetchModeMap.requestIdToTransactionIdMap.Add(Guid.Parse(request.Resp.RequestId), transactionId);
            }

            return Accepted();
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
                logger.LogInformation($"{{cmSuffix}} {{correlationId}} {{authCode}} {{transactionId}} {{requestId}}",
                    cmSuffix, correlationId,
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