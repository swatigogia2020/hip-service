namespace In.ProjectEKA.HipService.Link
{
    using System.Collections.Generic;
    using HipLibrary.Patient.Model.Request;

    public class LinkReference
    {
        public string ConsentManagerUserId { get; }
        
        public string ReferenceNumber { get; }
        
        public IEnumerable<CareContext> CareContexts { get; }

        public LinkReference(string consentManagerUserId, string referenceNumber, IEnumerable<CareContext> careContexts)
        {
            ConsentManagerUserId = consentManagerUserId;
            ReferenceNumber = referenceNumber;
            CareContexts = careContexts;
        }
    }
}