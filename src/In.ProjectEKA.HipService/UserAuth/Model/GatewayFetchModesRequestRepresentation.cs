using System;
using Newtonsoft.Json;

namespace In.ProjectEKA.HipService.UserAuth.Model
{
    public class GatewayFetchModesRequestRepresentation
    {
        public Guid requestId { get; }

        public DateTime timestamp { get; }

        public FetchQuery query { get; }

        public string cmSuffix { get; }

        public GatewayFetchModesRequestRepresentation(Guid requestId, DateTime timestamp, FetchQuery query,
            string cmSuffix)
        {
            this.requestId = requestId;
            this.timestamp = timestamp;
            this.query = query;
            this.cmSuffix = cmSuffix;
        }

        public string dump(Object o)
        {
            return JsonConvert.SerializeObject(o);
        }
    }
}