using System.Collections.Generic;
using In.ProjectEKA.HipLibrary.Patient.Model;

namespace In.ProjectEKA.HipService.Link.Model
{
    public class AddCareContextsPatient
    {
        public string referenceNumber { get; }
        public string display { get; }
        public List<CareContextRepresentation> careContexts { get; }

        public AddCareContextsPatient(string referenceNumber, string display, List<CareContextRepresentation> careContexts)
        {
            this.referenceNumber = referenceNumber;
            this.display = display;
            this.careContexts = careContexts;
        }
    }
}