namespace In.ProjectEKA.HipService.Patient.Model
{
    public class Profile
    {
        public string HipCode { get; }
        public PatientDemographics PatientDemographics { get; }
        public Profile(string hipCode, PatientDemographics patient)
        {
            HipCode = hipCode;
            PatientDemographics = patient;
        }
    }
}