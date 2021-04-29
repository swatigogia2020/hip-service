using System;
using Newtonsoft.Json;

namespace In.ProjectEKA.HipService.UserAuth.Model
{
    public class GatewayAuthInitRequestRepresentation
    {
        public Guid requestId { get; }

        public DateTime timestamp { get; }

        public AuthInitQuery query { get; }


        public GatewayAuthInitRequestRepresentation(Guid requestId, DateTime timestamp, AuthInitQuery query)
        {
            this.requestId = requestId;
            this.timestamp = timestamp;
            this.query = query;
        }

        public string dump(Object o)
        {
            return JsonConvert.SerializeObject(o);
        }
    }
}