using System;
using System.Threading.Tasks;
using In.ProjectEKA.HipLibrary.Patient.Model;
using In.ProjectEKA.HipService.Common;
using In.ProjectEKA.HipService.Common.Model;
using In.ProjectEKA.HipService.Gateway;
using In.ProjectEKA.HipService.Link.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace In.ProjectEKA.HipService.Link
{
    using static Constants;

    [ApiController]
    public class CareContextController : Controller
    {
        private readonly GatewayConfiguration gatewayConfiguration;
        private readonly BahmniConfiguration bahmniConfiguration;
        private readonly IGatewayClient gatewayClient;
        private readonly ILogger<CareContextController> logger;
        private readonly ICareContextService careContextService;

        public CareContextController(IGatewayClient gatewayClient,
            ILogger<CareContextController> logger,
            BahmniConfiguration bahmniConfiguration,
            GatewayConfiguration gatewayConfiguration,
            ICareContextService careContextService
        )
        {
            this.gatewayClient = gatewayClient;
            this.logger = logger;
            this.bahmniConfiguration = bahmniConfiguration;
            this.gatewayConfiguration = gatewayConfiguration;
            this.careContextService = careContextService;
        }

        [Route(PATH_ADD_CONTEXTS)]
        public async Task<ActionResult> AddContexts(
            [FromHeader(Name = CORRELATION_ID)] string correlationId, [FromBody] AddContextsRequest addContextsRequest)
        {
            var (gatewayAddContextsRequestRepresentation, error) =
                careContextService.AddContextsResponse(addContextsRequest);
            if (error != null)
                return StatusCode(StatusCodes.Status400BadRequest, error);
            Guid requestId = gatewayAddContextsRequestRepresentation.RequestId;
            var cmSuffix = gatewayConfiguration.CmSuffix;
            try
            {
                logger.Log(LogLevel.Information,
                    LogEvents.AddContext,
                    "Request for add-contexts to gateway: {@GatewayResponse}",
                    gatewayAddContextsRequestRepresentation.dump(gatewayAddContextsRequestRepresentation));
                await gatewayClient.SendDataToGateway(PATH_ADD_PATIENT_CONTEXTS,
                    gatewayAddContextsRequestRepresentation,
                    cmSuffix, correlationId);
                return Accepted();
            }
            catch (Exception exception)
            {
                logger.LogError(LogEvents.AddContext, exception, "Error happened for requestId: {RequestId} for" +
                                                                 " add-carecontexs request", requestId);
            }

            return StatusCode(StatusCodes.Status504GatewayTimeout,
                new ErrorRepresentation(new Error(ErrorCode.GatewayTimedOut, "Gateway timed out")));
        }
    }
}