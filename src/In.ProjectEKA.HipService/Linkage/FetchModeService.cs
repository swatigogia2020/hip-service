using System;
using In.ProjectEKA.HipService.Common;
using In.ProjectEKA.HipService.Gateway;


namespace In.ProjectEKA.HipService.Linkage
{
    using static Constants;

    public class FetchModeService : IFetchModeService
    {
        public virtual GatewayFetchModesRequestRepresentation FetchModeResponse(
            FetchRequest fetchRequest, GatewayConfiguration gatewayConfiguration)
        {
            Requester requester = new Requester(gatewayConfiguration.ClientId, FETCH_MODE_REQUEST_TYPE);
            FetchQuery query = new FetchQuery(fetchRequest.healthId, FETCH_MODE_PURPOSE, requester);
            DateTime timeStamp = DateTime.Now.ToUniversalTime();
            Guid requestId = Guid.NewGuid();
            return new GatewayFetchModesRequestRepresentation(requestId, timeStamp, query);
        }
    }
}