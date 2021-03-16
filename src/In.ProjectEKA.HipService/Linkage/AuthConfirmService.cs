using System;
using System.Threading.Tasks;
using Hl7.Fhir.Model;

namespace In.ProjectEKA.HipService.Linkage
{
    public abstract class AuthConfirmService
    {
        public virtual GatewayAuthConfirmRequestRepresentation authConfirmResponse(
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