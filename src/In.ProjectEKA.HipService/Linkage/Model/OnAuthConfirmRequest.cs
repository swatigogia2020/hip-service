using System;
using In.ProjectEKA.HipLibrary.Patient.Model;

namespace In.ProjectEKA.HipService.Linkage
{
    public class OnAuthConfirmRequest
    {
        public Guid requestID { get; }
        public DateTime timestamp { get; }
        public OnConfirmAuth auth { get; }
        public AuthConfirmPatient patient { get; }
        
        public Error error { get; }
        public Resp resp { get; }
            

        public OnAuthConfirmRequest(Guid requestId, DateTime timestamp, OnConfirmAuth auth, AuthConfirmPatient patient, Error error, Resp resp)
        {
            this.requestID = requestId;
            this.timestamp = timestamp;
            this.auth = auth;
            this.patient = patient;
            this.error = error;
            this.resp = resp;
        }
    }
}