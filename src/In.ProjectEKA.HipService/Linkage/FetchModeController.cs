namespace In.ProjectEKA.HipService.Linkage
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Gateway;
    using Gateway.Model;
    using static Common.Constants;
    using Hangfire;
    using HipLibrary.Patient.Model;
    using Logger;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.Extensions.Logging;
    using Common;

public class FetchModeController : Controller{

    private readonly IGatewayClient gatewayClient;

    private readonly IBackgroundJobClient backgroundJob;
    private readonly ILogger<CareContextDiscoveryController> logger;


    public FetchModeController(IGatewayClient gatewayClient, IBackgroundJobClient backgroundJob, ILogger<FetchModeController> logger){
            this.gatewayClient = gatewayClient;
            this.backgroundJob = backgroundJob;
            this.logger = logger;
    }

    [ApiController]
    [HttpPost(FETCH_MODES)]
    public async Task FetchPatientsAuthModes (string healthid) {

        FromHeader(Name = CORRELATION_ID)] string correlationId;
        string cmsuffix = appsettings.gateway.cmsuffix;
        Requester requester = new requester(appsettings.gateway.clientId, FETCH_MODE_REQUEST_TYPE);
        FetchQuery query = new query(healthid, FETCH_MODE_PURPOSE, requester );
        String timeStamp = GetTimestamp(DateTime.Now);
        Guid requestId = Guid.NewGuid();
        GatewayFetchModesRequestRepresentation gr = new GatewayFetchModesRequestRepresentation( requestId, timestamp, query);


        try{
            await gatewayClient.SendDataToGateway (PATH_FETCH_AUTH_MODES, gr,cmSuffix, correlationId) 
            //requestmap.add(reqId, [""]); return if the reqid ia lready in the map
            int i=0;
            do{
                System.Threading.Thread.Sleep(2000);
                if(FetchModeMap.requestIdToFetchMode.ContainsKey(requestId).value != null){
                    return FetchModeMap.requestIdToFetchMode[requestId];
                }
                i++;

            } while (i < 5);
            throw new TimeoutException("Timeout for request_id: "+ requestId);
        }
        catch(Exception exception){
            logger.LogError(LogEvents.Discovery, exception, "Error happened for {RequestId}", requestId);
        }
    
    }

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
                string authModes = "";
                foreach (Mode mode in request.Auth.Modes)
                {
                    authModes += mode + ",";
                }

                authModes = authModes.Remove(authModes.Length - 1, 1);
                FetchModeMap.requestIdToFetchMode.Add(request.RequestId, authModes);
                Log.Information($" Auth Purpose:{request.Auth.Purpose},");
                Log.Information($" Auth Modes:{authModes}.");
            }

            Log.Information($" Resp RequestId:{request.Resp.RequestId}");
            return Accepted();
}
}
}



