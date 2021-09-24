namespace In.ProjectEKA.HipService.Patient.Model
{
    public class PatientNotifyAcknowledgement
    {
        public PatientNotifyAcknowledgement(string status)
        {
            Status = status;
        }

        public string Status { get; }
    }
}