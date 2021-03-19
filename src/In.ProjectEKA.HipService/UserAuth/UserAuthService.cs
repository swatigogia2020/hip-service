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
            var patientIdSplit = fetchRequest.healthId.Split("@");
            if (patientIdSplit.Length == 2)
                cmSuffix = patientIdSplit[1];
            var requester = new Requester(gatewayConfiguration.ClientId, FETCH_MODE_REQUEST_TYPE);
            var purpose = fetchRequest.purpose;
            var query = purpose != null
                ? new FetchQuery(fetchRequest.healthId, purpose, requester)
                : new FetchQuery(fetchRequest.healthId, requester);
            var timeStamp = DateTime.Now.ToUniversalTime();
            var requestId = Guid.NewGuid();
            return new GatewayFetchModesRequestRepresentation(requestId, timeStamp, query, cmSuffix);
        }

        public virtual GatewayAuthInitRequestRepresentation AuthInitResponse(
            AuthInitRequest authInitRequest, GatewayConfiguration gatewayConfiguration)
        {
            var timeStamp = DateTime.Now.ToUniversalTime();
            var requestId = Guid.NewGuid();
            var requester = new Requester(gatewayConfiguration.ClientId, FETCH_MODE_REQUEST_TYPE);
            var purpose = authInitRequest.purpose;
            var authInitQuery = purpose != null
                ? new AuthInitQuery(authInitRequest.healthId, purpose, authInitRequest.authMode, requester)
                : new AuthInitQuery(authInitRequest.healthId, authInitRequest.authMode, requester);
            return new GatewayAuthInitRequestRepresentation(requestId, timeStamp, authInitQuery, cmSuffix);
        }

        public virtual GatewayAuthConfirmRequestRepresentation AuthConfirmResponse(
            AuthConfirmRequest authConfirmRequest)
        {
            var credential = new AuthConfirmCredential(authConfirmRequest.authCode);
            var transactionId = authConfirmRequest.transactionId;
            var timeStamp = DateTime.Now.ToUniversalTime();
            var requestId = Guid.NewGuid();
            return new GatewayAuthConfirmRequestRepresentation(requestId, timeStamp, transactionId, credential,
                cmSuffix);
        }
    }
}