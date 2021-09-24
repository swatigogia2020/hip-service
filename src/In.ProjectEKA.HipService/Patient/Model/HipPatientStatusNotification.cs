using System;
using In.ProjectEKA.HipService.Consent.Model;

namespace In.ProjectEKA.HipService.Patient.Model
{
    public class HipPatientStatusNotification
    {
        public Guid requestId { get; }
        public DateTime timestamp { get; }

        public PatientNotification notification { get; }

        public HipPatientStatusNotification(Guid requestId, DateTime timestamp, PatientNotification notification)
        {
            this.requestId = requestId;
            this.timestamp = timestamp;
            this.notification = notification;
        }
    }
}