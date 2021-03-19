using System;
using Newtonsoft.Json;

namespace In.ProjectEKA.HipService.UserAuth.Model
{
    public class GatewayAuthConfirmRequestRepresentation
    {
        public Guid requestId { get; }
        public DateTime timestamp { get; }
        public string transactionId { get; }
        public AuthConfirmCredential credential { get; }

        public string cmSuffix { get; }

        public GatewayAuthConfirmRequestRepresentation(Guid requestId, DateTime timestamp, string transactionId,
            AuthConfirmCredential credential, string cmSuffix)
        {
            this.requestId = requestId;
            this.transactionId = transactionId;
            this.timestamp = timestamp;
            this.credential = credential;
            this.cmSuffix = cmSuffix;
        }

        public string dump(Object o)
        {
            return JsonConvert.SerializeObject(o);
        }
    }
}