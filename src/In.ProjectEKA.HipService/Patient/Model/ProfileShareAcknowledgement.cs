namespace In.ProjectEKA.HipService.Patient.Model
{
    public class ProfileShareAcknowledgement
    {
        public ProfileShareAcknowledgement(string status,string healthId)
        {
            Status = status;
            HealthId = healthId;
        }
        public string Status { get; }
        public string HealthId { get; }
    }
}