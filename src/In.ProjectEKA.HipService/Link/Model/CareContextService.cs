using System;
using In.ProjectEKA.HipLibrary.Patient.Model;
using In.ProjectEKA.HipService.Common.Model;

namespace In.ProjectEKA.HipService.Link.Model
{
    public class CareContextService : ICareContextService
    {
        public Tuple<GatewayAddContextsRequestRepresentation, ErrorRepresentation> AddContextsResponse(
            AddContextsRequest addContextsRequest, BahmniConfiguration bahmniConfiguration)
        {
            var accessToken = addContextsRequest.accessToken;
            var referenceNumber = addContextsRequest.referenceNumber;
            var careContexts = addContextsRequest.careContexts;
            var display = addContextsRequest.display;
            var patient = new AddCareContextsPatient(referenceNumber, display, careContexts);
            var link = new AddCareContextsLink(accessToken, patient);
            var timeStamp = DateTime.Now.ToUniversalTime();
            var requestId = Guid.NewGuid();
            return new Tuple<GatewayAddContextsRequestRepresentation, ErrorRepresentation>
                (new GatewayAddContextsRequestRepresentation(requestId, timeStamp, link), null);
        }
    }
}