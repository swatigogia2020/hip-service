using System;
using In.ProjectEKA.HipService.Gateway;
using In.ProjectEKA.HipService.UserAuth.Model;
using static In.ProjectEKA.HipService.Common.Constants;

namespace In.ProjectEKA.HipService.UserAuth
{
    public class UserAuthService : IUserAuthService
    {
        private static string cmSuffix;

        public virtual GatewayFetchModesRequestRepresentation FetchModeResponse(
            FetchRequest fetchRequest, GatewayConfiguration gatewayConfiguration)
        {
            var patientId = fetchRequest.healthId.Split("@");
            if (patientId.Length == 2)
                cmSuffix = patientId[1];
            Requester requester = new Requester(gatewayConfiguration.ClientId, FETCH_MODE_REQUEST_TYPE);
            FetchQuery query = new FetchQuery(fetchRequest.healthId, FETCH_MODE_PURPOSE, requester);
            DateTime timeStamp = DateTime.Now.ToUniversalTime();
            Guid requestId = Guid.NewGuid();
            return new GatewayFetchModesRequestRepresentation(requestId, timeStamp, query, cmSuffix);
        }

        public virtual GatewayAuthInitRequestRepresentation AuthInitResponse(
            AuthInitRequest authInitRequest, GatewayConfiguration gatewayConfiguration)
        {
            DateTime timeStamp = DateTime.Now.ToUniversalTime();
            Guid requestId = Guid.NewGuid();
            Requester requester = new Requester(gatewayConfiguration.ClientId, FETCH_MODE_REQUEST_TYPE);
            AuthInitQuery query = new AuthInitQuery(authInitRequest.healthId, FETCH_MODE_PURPOSE,
                authInitRequest.authMode, requester);
            return new GatewayAuthInitRequestRepresentation(requestId, timeStamp, query, cmSuffix);
        }

        public virtual GatewayAuthConfirmRequestRepresentation AuthConfirmResponse(
            AuthConfirmRequest authConfirmRequest)
        {
            AuthConfirmCredential credential = new AuthConfirmCredential(authConfirmRequest.authCode);
            string transactionId = authConfirmRequest.transactionId;
            DateTime timeStamp = DateTime.Now.ToUniversalTime();
            Guid requestId = Guid.NewGuid();
            return new GatewayAuthConfirmRequestRepresentation(requestId, timeStamp, transactionId, credential,
                cmSuffix);
        }
    }
}