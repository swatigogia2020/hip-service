using System.Threading.Tasks;
using System.Web;
using In.ProjectEKA.HipService.Common;
using In.ProjectEKA.HipService.OpenMrs;
using Serilog;
using System.Text.Json;
using System;

namespace In.ProjectEKA.HipService.DataFlow
{
    public class OpenMrsPatientData : IOpenMrsPatientData

    {
        private readonly IOpenMrsClient openMrsClient;

        public OpenMrsPatientData(IOpenMrsClient openMrsClient)
        {
            this.openMrsClient = openMrsClient;
        }

        public async Task<string> GetPatientData(string patientUuid, string grantedContext, string toDate,
            string fromDate, string hiType)
        {
            switch (hiType)
            {
                case "prescription":
                    return await getPrescription(patientUuid, grantedContext, toDate, fromDate);
                default:
                    Log.Error("Invalid HiType");
                    return "";
            }
        }

        private string pathForOpenMRSMedication(string patientUuuid, string grantedContext)
        {
            var pathForMedication = $"{Constants.OPENMRS_MEDICATION}";
            var query = HttpUtility.ParseQueryString(string.Empty);
            if (!string.IsNullOrEmpty(patientUuuid))
            {
                query["patientId"] = patientUuuid;
                query["visitType"] = grantedContext;
            }

            if (query.ToString() != "")
            {
                pathForMedication = $"{pathForMedication}/?{query}";
            }

            return pathForMedication;
        }

        private async Task<string> getPrescription(string consentId, string grantedContext, string toDate,
            string fromDate)
        {
            var pathForPrescription = $"{Constants.OPENMRS_PRESCRIPTION}";
            var query = HttpUtility.ParseQueryString(string.Empty);
            if (
                !string.IsNullOrEmpty(consentId) &&
                !string.IsNullOrEmpty(grantedContext) &&
                !string.IsNullOrEmpty(toDate) &&
                !string.IsNullOrEmpty(fromDate)
            )
            {
                query["patientId"] = consentId;
                query["visitType"] = grantedContext;
                query["fromDate"] = fromDate;
                query["toDate"] = DateTime.Parse(toDate).AddDays(2).ToString("yyyy-MM-dd");
            }
            if (query.ToString() != "")
            {
                pathForPrescription = $"{pathForPrescription}/?{query}";
            }
            Log.Information("OMOD endpoint being called: " + pathForPrescription);
            var response = await openMrsClient.GetAsync(pathForPrescription);
            var content = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(content);
            var root = jsonDoc.RootElement;
            var prescriptions = "prescriptions";
            if (root.GetProperty(prescriptions).GetArrayLength() > 0)
            {
                return root.GetProperty(prescriptions)[0].GetProperty("bundle").ToString();
            }
            return "";
        }
    }
}