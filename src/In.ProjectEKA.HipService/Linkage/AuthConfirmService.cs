using System;

namespace In.ProjectEKA.HipService.Linkage
{
    public class AuthConfirmService : IAuthConfirmService
    {
        public virtual GatewayAuthConfirmRequestRepresentation AuthConfirmResponse(
            AuthConfirmRequest authConfirmRequest)
        {
            AuthConfirmCredential credential = new AuthConfirmCredential(authConfirmRequest.authCode);
            string transactionId = authConfirmRequest.transactionId;
            DateTime timeStamp = DateTime.Now.ToUniversalTime();
            Guid requestId = Guid.NewGuid();
          return new GatewayAuthConfirmRequestRepresentation(requestId, timeStamp, transactionId, credential);
        }
    }
}