namespace In.ProjectEKA.HipService.Patient.Model
{
    public class PatientName
    {
        public PatientName(string givenName,string middleName, string familyName)
        {
            this.givenName = givenName;
            this.familyName = familyName;
            this.middleName = middleName;
        }
        public string givenName { get; set; }
        public string middleName { get; set; }
        public string familyName { get; set; }
    }
}