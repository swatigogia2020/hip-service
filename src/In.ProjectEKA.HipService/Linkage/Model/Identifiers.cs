using Hl7.Fhir.Model;

namespace In.ProjectEKA.HipService.Linkage
{
    public class Identifiers
    {
        public string type { get; }
        public string value { get; }

        public Identifiers(string type, string value)
        {
            this.type = type;
            this.value = value;
        }
    }
}