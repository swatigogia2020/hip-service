using System.Threading.Tasks;
using System.Web;
using In.ProjectEKA.HipService.Common;
using In.ProjectEKA.HipService.OpenMrs;
using Serilog;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using In.ProjectEKA.HipService.Common.Model;

namespace In.ProjectEKA.HipService.DataFlow
{
    public class OpenMrsPatientData : IOpenMrsPatientData
    {
        private readonly Dictionary<string, string> hiTypeToRootElement = new Dictionary<string, string>()
        {
            {HiType.Prescription.ToString().ToLower(), "prescriptions"},
            {HiType.DiagnosticReport.ToString().ToLower(), "diagnosticReports"}
        };

        private readonly IOpenMrsClient openMrsClient;

        public OpenMrsPatientData(IOpenMrsClient openMrsClient)
        {
            this.openMrsClient = openMrsClient;
        }

        private static bool IsValidProgramCareContext(string careContextName)
        {
            string pattern = @"\(ID Number:(\d+)\)";
            return Regex.Match(careContextName, pattern).Success;
        }

        public async Task<string> GetPatientData(string patientUuid, string careContextReference, string toDate,
            string fromDate, string hiType)
        {
            if (!hiTypeToRootElement.ContainsKey(hiType)) return "";
            if (!IsValidProgramCareContext(careContextReference))
                return await GetForVisits(hiType, patientUuid, careContextReference, toDate, fromDate);
            var programName = careContextReference
                .Substring(0, careContextReference.IndexOf("(", StringComparison.Ordinal))
                .Trim();
            var indexOfClosingBracket = careContextReference.IndexOf(")", StringComparison.Ordinal);
            var indexOfColon = careContextReference.IndexOf(":", StringComparison.Ordinal);
            var programId = careContextReference
                .Substring(indexOfColon + 1, indexOfClosingBracket - indexOfColon - 1)
                .Trim();

            return await GetForPrograms(hiType, patientUuid, programName, programId, toDate, fromDate);
        }

        private async Task<string> GetForVisits(string hiType, string consentId, string grantedContext, string toDate,
            string fromDate)
        {
            var pathForVisit = $"{Constants.PATH_OPENMRS_HITYPE}{hiTypeToRootElement[hiType]}/visit/";
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
                query["toDate"] = DateTime.Parse(toDate).AddDays(1).ToString("yyyy-MM-dd");
            }

            if (query.ToString() != "")
            {
                pathForVisit = $"{pathForVisit}?{query}";
            }

            Log.Information("VISIT endpoint being called: " + pathForVisit);
            var response = await openMrsClient.GetAsync(pathForVisit);
            if (response == null) return "";
            var content = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(content);
            var root = jsonDoc.RootElement;
            if (root.GetProperty(hiTypeToRootElement[hiType]).GetArrayLength() > 0)
            {
                return root.GetProperty(hiTypeToRootElement[hiType])[0].GetProperty("bundle").ToString();
            }

            return "";
        }

        private async Task<string> GetForPrograms(string hiType, string consentId, string programName, string programId,
            string toDate,
            string fromDate)
        {
            var pathForProgram = $"{Constants.PATH_OPENMRS_HITYPE}{hiTypeToRootElement[hiType]}/program/";
            var query = HttpUtility.ParseQueryString(string.Empty);
            if (
                !string.IsNullOrEmpty(consentId) &&
                !string.IsNullOrEmpty(programName) &&
                !string.IsNullOrEmpty(toDate) &&
                !string.IsNullOrEmpty(fromDate)
            )
            {
                query["patientId"] = consentId;
                query["programName"] = programName;
                query["programEnrollmentId"] = programId;
                query["fromDate"] = fromDate;
                query["toDate"] = toDate;
            }

            if (query.ToString() != "")
            {
                pathForProgram = $"{pathForProgram}?{query}";
            }

            Log.Information("PROGRAM endpoint being called: " + pathForProgram);
            var response = await openMrsClient.GetAsync(pathForProgram);
            var content = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(content);
            var root = jsonDoc.RootElement;
            if (root.GetProperty(hiTypeToRootElement[hiType]).GetArrayLength() > 0)
            {
                return root.GetProperty(hiTypeToRootElement[hiType])[0].GetProperty("bundle").ToString();
            }

            return "";
        }
    }
}