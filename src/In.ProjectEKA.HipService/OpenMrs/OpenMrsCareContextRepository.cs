using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using In.ProjectEKA.HipLibrary.Patient;
using In.ProjectEKA.HipLibrary.Patient.Model;
using Log = In.ProjectEKA.HipService.Logger.Log;

namespace In.ProjectEKA.HipService.OpenMrs
{
    public class OpenMrsCareContextRepository : ICareContextRepository
    {
        private readonly IOpenMrsClient openMrsClient;

        public OpenMrsCareContextRepository(IOpenMrsClient openMrsClient)
        {
            this.openMrsClient = openMrsClient;
        }

        public async Task<IEnumerable<CareContextRepresentation>> GetCareContexts(string patientUuid)
        {
            var path = DiscoveryPathConstants.OnCareContextPath;
            var query = HttpUtility.ParseQueryString(string.Empty);
            if (!string.IsNullOrEmpty(patientUuid))
            {
                query["patientUuid"] = patientUuid;
            }

            if (query.ToString() != "")
            {
                path = $"{path}?{query}";
            }

            var response = await openMrsClient.GetAsync(path);
            var content = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(content);
            var root = jsonDoc.RootElement;
            var combinedCareContexts = new List<CareContextRepresentation>();
            var visitCareContexts = new List<CareContextRepresentation>();
            var programCareContexts = new List<CareContextRepresentation>();
            
            for (int i = 0; i < root.GetArrayLength(); i++)
            {
                var careContextType = TryGetProperty(root[i],"careContextType").GetString();

                switch (careContextType)
                {
                    case "VISIT_TYPE":
                        visitCareContexts.Add(Visits(root[i]));
                        break;

                    case "PROGRAM":
                        programCareContexts.Add(Programs(root[i]));
                        break;
                }
            }

            combinedCareContexts.AddRange(visitCareContexts);
            combinedCareContexts.AddRange(programCareContexts);

            return combinedCareContexts;
        }

        private CareContextRepresentation Programs(JsonElement root)
        {
            var careContextName = root.GetProperty("careContextName").GetString();
            var careContextReferenceNumber = root.GetProperty("careContextReference").ToString();
            return new CareContextRepresentation(careContextReferenceNumber, careContextName);
        }

        private CareContextRepresentation Visits(JsonElement root)
        {
            var careContextName = root.GetProperty("careContextName").GetString();
            var careContextReferenceNumber = root.GetProperty("careContextReference").ToString();
            return new CareContextRepresentation(careContextName, null);
        }

        private JsonElement TryGetProperty(JsonElement data, string propertyName)
        {
            if (!data.TryGetProperty(propertyName, out var property))
            {
                LogAndThrowException($"Property '{propertyName}' is missing when getting program enrollments.");
            }

            return property;
        }

        private void LogAndThrowException(string message)
        {
            Log.Error(message);
            throw new OpenMrsFormatException();
        }
    }
}