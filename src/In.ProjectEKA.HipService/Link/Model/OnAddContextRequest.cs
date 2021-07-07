using System;
using Elastic.CommonSchema;
using Error = In.ProjectEKA.HipLibrary.Patient.Model.Error;

namespace In.ProjectEKA.HipService.Link.Model
{
    public class OnAddContextRequest
    {
        public Guid requestId { get; }

        public DateTime timestamp { get; }

        public OnAddContextAcknowledgement acknowledgement { get; }

        public Error error { get; }
        public OnAddContextResp resp { get; }

        public OnAddContextRequest(Guid requestId, DateTime timestamp, OnAddContextAcknowledgement acknowledgement,
            Error error, OnAddContextResp resp)
        {
            this.requestId = requestId;
            this.timestamp = timestamp;
            this.acknowledgement = acknowledgement;
            this.error = error;
            this.resp = resp;
        }
    }
}