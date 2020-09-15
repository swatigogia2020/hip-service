using System;
using System.Threading.Tasks;
using System.Web;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using In.ProjectEKA.HipLibrary.DataFlow;

namespace In.ProjectEKA.HipService.OpenMrs
{
    public class OpenMrsDataFlowRepository : IOpenMrsDataFlowRepository
    {
        private readonly IOpenMrsClient openMrsClient;

        public OpenMrsDataFlowRepository(IOpenMrsClient openMrsClient)
        {
            this.openMrsClient = openMrsClient;
        }

        public async Task<Bundle> GetBundleForCareContext(string patientId, string linkedCareContextVisitType)
        {
            var path = DiscoveryPathConstants.OnCareCotextBundlePath;
            var query = HttpUtility.ParseQueryString(string.Empty);
            if (!string.IsNullOrEmpty(patientId))
            {
                query["patientId"] = patientId;
                query["visitType"] = linkedCareContextVisitType;
            }
            if (query.ToString() != "")
            {
                path = $"{path}?{query}";
            }

            var response = await openMrsClient.GetAsync(path);
            var content = await response.Content.ReadAsStringAsync();

            var bundle = new FhirJsonParser().Parse<Bundle>(content);

            return bundle;
        }
    }
}
