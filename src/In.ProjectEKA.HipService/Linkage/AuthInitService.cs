using System;
using In.ProjectEKA.HipService.Common;
using In.ProjectEKA.HipService.Gateway;


namespace In.ProjectEKA.HipService.Linkage
{
    using static Constants;

    public class AuthInitService : IAuthInitService
    {
        public GatewayAuthInitRequestRepresentation AuthInitResponse(
            AuthInitRequest authInitRequest, GatewayConfiguration gatewayConfiguration)
        {
            DateTime timeStamp = DateTime.Now.ToUniversalTime();
            Guid requestId = Guid.NewGuid();
            Requester requester = new Requester(gatewayConfiguration.ClientId, FETCH_MODE_REQUEST_TYPE);
            AuthInitQuery query = new AuthInitQuery(authInitRequest.healthId, FETCH_MODE_PURPOSE,
                authInitRequest.authMode, requester);
            return new GatewayAuthInitRequestRepresentation(requestId, timeStamp, query);
        }
    }
}