using System.Collections.Generic;
using In.ProjectEKA.HipLibrary.Patient.Model;

namespace In.ProjectEKA.HipService.Link.Model
{
    public class AddContextsRequest
    {
        public string ReferenceNumber { get; }
        public string Display { get; }
        public List<CareContextRepresentation> CareContexts { get; }

        public AddContextsRequest(string accessToken, string referenceNumber, List<CareContextRepresentation> careContexts,
            string display)
        {
            ReferenceNumber = referenceNumber;
            CareContexts = careContexts;
            Display = display;
        }
    }
}