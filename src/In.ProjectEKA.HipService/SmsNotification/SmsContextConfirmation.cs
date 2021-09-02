using System;
using In.ProjectEKA.HipLibrary.Patient.Model;
using In.ProjectEKA.HipService.SmsNotification.Model;

namespace In.ProjectEKA.HipService.SmsNotification
{
    public class SmsContextConfirmation
    {
        public SmsContextConfirmation(string requestId, DateTime timestamp,
                    SmsNotificationAcknowledgement acknowledgement, Error error, Resp resp)
                {
                    RequestId = requestId;
                    Timestamp = timestamp;
                    Acknowledgement = acknowledgement;
                    Error = error;
                    Resp = resp;
                }
        
                public string RequestId { get; }
                public DateTime Timestamp { get; }
                public Error Error { get; }
                public Resp Resp { get; }
                public SmsNotificationAcknowledgement Acknowledgement { get; }
    }
}