using System;
using In.ProjectEKA.HipService.Common;
using In.ProjectEKA.HipService.Gateway;


namespace In.ProjectEKA.HipService.Linkage
{
    using static Constants;

    public abstract class AuthInitService
    {
        private readonly GatewayConfiguration _gatewayConfiguration;

        protected AuthInitService(GatewayConfiguration gatewayConfiguration)
        {
            _gatewayConfiguration = gatewayConfiguration;
        }

        public virtual GatewayAuthInitRequestRepresentation AuthInitResponse(
            AuthInitRequest authInitRequest)
        {
            DateTime timeStamp = DateTime.Now.ToUniversalTime();
            Guid requestId = Guid.NewGuid();
            Requester requester = new Requester(_gatewayConfiguration.ClientId, FETCH_MODE_REQUEST_TYPE);
            AuthInitQuery query = new AuthInitQuery(authInitRequest.healthId, FETCH_MODE_PURPOSE,
                authInitRequest.authMode, requester);
            return new GatewayAuthInitRequestRepresentation(requestId, timeStamp, query);
        }
    }
}