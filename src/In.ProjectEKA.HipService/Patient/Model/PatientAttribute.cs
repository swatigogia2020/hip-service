namespace In.ProjectEKA.HipService.Patient.Model
{
    public class PatientAttribute
    {
        public PatientAttribute(AttributeType attributeType, string value)
        {
            this.attributeType = attributeType;
            this.value = value;
        }
        public AttributeType attributeType { get; }
        public string value { get; }
    }
}