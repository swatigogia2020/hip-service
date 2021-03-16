using System;
using In.ProjectEKA.HipLibrary.Patient.Model;

namespace In.ProjectEKA.HipService.Link.Model
{
    public class OnFetchAuthModeRequest
    {
        public OnFetchAuthModeRequest(Guid requestId, DateTime timestamp, AuthModeFetch auth, Error error, Resp resp)
        {
            RequestId = requestId;
            Timestamp = timestamp;
            Auth = auth;
            Error = error;
            Resp = resp;
        }

        public Guid RequestId { get; }
        public DateTime Timestamp { get; }
        public AuthModeFetch Auth { get; }
        public Error Error { get; }
        public Resp Resp { get; }
    }
}