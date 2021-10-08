namespace In.ProjectEKA.HipService.Patient.Model
{
    public class ExtraPatientIdentifier
    {
        public ExtraPatientIdentifier(string identifier, string identifierType)
        {
            this.identifier = identifier;
            this.identifierType = identifierType;
        }
        public string identifierType { get; set; }
        public string identifier { get; set; }
    }
}