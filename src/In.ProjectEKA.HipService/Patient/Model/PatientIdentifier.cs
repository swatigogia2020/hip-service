namespace In.ProjectEKA.HipService.Patient.Model
{
    public class PatientIdentifier
    {
        public PatientIdentifier(string identifierSourceUuid, string identifierPrefix, string identifierType)
        {
            this.identifierSourceUuid = identifierSourceUuid;
            this.identifierPrefix = identifierPrefix;
            this.identifierType = identifierType;
        }
        public string identifierSourceUuid { get; set; }
        public string identifierPrefix { get; set; }
        public string identifierType { get; set; }
    }
}