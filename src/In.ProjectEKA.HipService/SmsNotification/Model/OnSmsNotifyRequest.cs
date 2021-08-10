using System;
using In.ProjectEKA.HipLibrary.Patient.Model;

namespace In.ProjectEKA.HipService.SmsNotification.Model
{
    public class OnSmsNotifyRequest
    {
        public Guid requestId { get; }
        public DateTime timestamp { get; }
        public string status { get; }
        public Error error { get; }
        public Resp resp { get; }

        public OnSmsNotifyRequest(Guid requestId, DateTime timestamp, string status, Error error, Resp resp)
        {
            this.requestId = requestId;
            this.timestamp = timestamp;
            this.status = status;
            this.error = error;
            this.resp = resp;
        }
    }
}