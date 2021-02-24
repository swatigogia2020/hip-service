using Newtonsoft.Json;

namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    public class CareContextRepresentation
    {
        [JsonConstructor]
        public CareContextRepresentation(string referenceNumber, string display)
        {
            ReferenceNumber = referenceNumber;
            Display = display;
        }
        public CareContextRepresentation(string referenceNumber, string display, string type)
        {
            ReferenceNumber = referenceNumber;
            Display = display;
            Type = type;
        }

        public string ReferenceNumber { get; }

        public string Display { get; }
        public string Type { get; }
    }
}