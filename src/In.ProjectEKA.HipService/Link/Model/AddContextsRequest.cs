using System.Collections.Generic;
using In.ProjectEKA.HipLibrary.Patient.Model;

namespace In.ProjectEKA.HipService.Link.Model
{
    public class AddContextsRequest
    {
        public string accessToken { get; }
        public string referenceNumber { get; }
        public string display { get; }
        public List<CareContextRepresentation> careContexts { get; }

        public AddContextsRequest(string accessToken, string referenceNumber, List<CareContextRepresentation> careContexts,
            string display)
        {
            this.accessToken = accessToken;
            this.referenceNumber = referenceNumber;
            this.careContexts = careContexts;
            this.display = display;
        }
    }
}