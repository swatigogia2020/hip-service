using System;
using Newtonsoft.Json;

namespace In.ProjectEKA.HipService.Link.Model
{
    public class GatewayAddContextsRequestRepresentation
    {
        public Guid requestId { get; }
        public DateTime timestamp { get; }
        public AddCareContextsLink link { get; }

        public GatewayAddContextsRequestRepresentation(Guid requestId, DateTime timestamp, AddCareContextsLink link)
        {
            this.requestId = requestId;
            this.timestamp = timestamp;
            this.link = link;
        }

        public string dump(Object o)
        {
            return JsonConvert.SerializeObject(o);
        }
    }
}