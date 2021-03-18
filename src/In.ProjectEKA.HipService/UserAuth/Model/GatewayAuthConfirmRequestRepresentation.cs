using System;

namespace In.ProjectEKA.HipService.UserAuth.Model
{
    public class GatewayAuthConfirmRequestRepresentation
    {
        public Guid requestId { get; }
        public DateTime timestamp { get; }
        public string transactionId { get; }
        public AuthConfirmCredential credential { get; }

        public GatewayAuthConfirmRequestRepresentation(Guid requestId, DateTime timestamp, string transactionId,
            AuthConfirmCredential credential)
        {
            this.requestId = requestId;
            this.transactionId = transactionId;
            this.timestamp = timestamp;
            this.credential = credential;
        }
        
    }
}