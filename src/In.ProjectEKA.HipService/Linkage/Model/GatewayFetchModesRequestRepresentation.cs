using System;
using Newtonsoft.Json;

namespace In.ProjectEKA.HipService.Linkage
{
    public class GatewayFetchModesRequestRepresentation
    {
        public Guid requestId { get; }

        public DateTime timestamp { get; }

        public FetchQuery query { get; }

        public GatewayFetchModesRequestRepresentation(Guid requestId, DateTime timestamp, FetchQuery query)
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