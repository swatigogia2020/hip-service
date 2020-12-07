using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Utility;
using In.ProjectEKA.HipLibrary.Patient.Model;

namespace In.ProjectEKA.HipService.DataFlow
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Optional;
    using Serilog;

    public class CollectHipService : ICollectHipService
    {
        private readonly IOpenMrsPatientData openMrsPatientData;

        public CollectHipService(IOpenMrsPatientData openMrsPatientData)
        {
            this.openMrsPatientData = openMrsPatientData;
        }

        public async Task<Option<Entries>> CollectData(TraceableDataRequest dataRequest)
        {
            var bundles = new List<CareBundle>();
            var patientData = await FindPatientData(dataRequest);
            var careContextReferences = patientData.Keys.ToList();
            foreach (var careContextReference in careContextReferences)
            {
                foreach (var result in patientData.GetOrDefault(careContextReference))
                {
                    Log.Information($"Returning file: {result}");
                    var bundle = new FhirJsonParser().Parse<Bundle>(result);
                    bundles.Add(new CareBundle(careContextReference, bundle));
                }
            }

            var entries = new Entries(bundles);
            return Option.Some(entries);
        }

        private static bool WithinRange(DateRange range, DateTime date)
        {
            var fromDate = ParseDate(range.From).AddHours(2);
            var toDate = ParseDate(range.To);
            return date > fromDate && date < toDate;
        }

        private static DateTime ParseDate(string dateString)
        {
            var formatStrings = new[]
            {
                "yyyy-MM-dd"/*, "yyyy-MM-dd hh:mm:ss", "yyyy-MM-dd hh:mm:ss tt", "yyyy-MM-ddTHH:mm:ss.fffzzz",
                "yyyy-MM-dd'T'HH:mm:ss.fff", "yyyy-MM-dd'T'HH:mm:ss.ffff", "yyyy-MM-dd'T'HH:mm:ss.fffff",
                "yyyy-MM-dd'T'HH:mm:ss", "yyyy-MM-dd'T'HH:mm:ss.ff", "yyyy-MM-dd'T'HH:mm:ss.ff",
                "dd/MM/yyyy", "dd/MM/yyyy hh:mm:ss", "dd/MM/yyyy hh:mm:ss tt", "dd/MM/yyyyTHH:mm:ss.fffzzz",
                "yyyy-MM-dd'T'HH:mm:ss.ffffff", "yyyy-MM-dd'T'HH:mm:ss.fff'Z'"*/
            };
            var tryParseExact = DateTime.TryParseExact(dateString,
                formatStrings,
                CultureInfo.CurrentCulture,
                DateTimeStyles.None,
                out var aDateTime);
            if (!tryParseExact)
                Log.Error($"Error parsing date: {dateString}");

            return aDateTime;
        }

        private async Task<Dictionary<string, List<string>>> FindPatientData(TraceableDataRequest request)
        {
            try
            {
                LogDataRequest(request);    
                var toDate = request.DateRange.To;
                var fromDate = request.DateRange.From;
                var careContextsAndListOfDataFiles = new Dictionary<string, List<string>>();
                foreach (var grantedContext in request.CareContexts) 
                {
                    var listOfDataFiles = new List<string>();
                    foreach (var hiType in request.HiType)
                    {
                        var hiTypeStr = hiType.ToString().ToLower();
                        var dataFiles =   openMrsPatientData
                            .GetPatientData(request.PatientUuid, grantedContext.CareContextReference, toDate, fromDate,
                                hiTypeStr).Result;
                        if (dataFiles.Length > 0) 
                            listOfDataFiles.Add(dataFiles);
                    }

                    careContextsAndListOfDataFiles.Add(grantedContext.CareContextReference, listOfDataFiles);
                }

                return careContextsAndListOfDataFiles;
            }
            catch (Exception e)
            {
                Log.Error("Error Occured while collecting data. {Error}", e);
            }

            return new Dictionary<string, List<string>>();
        }

        private static void LogDataRequest(TraceableDataRequest request)
        {
            var ccList = JsonConvert.SerializeObject(request.CareContexts);
            var requestedHiTypes = string.Join(", ", request.HiType.Select(hiType => hiType.ToString()));
            Log.Information("Data request received." +
                            $" transactionId:{request.TransactionId} , " +
                            $"CareContexts:{ccList}, " +
                            $"HiTypes:{requestedHiTypes}," +
                            $" From date:{request.DateRange.From}," +
                            $" To date:{request.DateRange.To}, " +
                            $"CallbackUrl:{request.DataPushUrl}, " +
                            $"PatientUuid:{request.PatientUuid}");
        }
    }
}