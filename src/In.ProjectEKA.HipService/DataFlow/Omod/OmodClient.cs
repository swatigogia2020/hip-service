using System;
using System.Net.Http;
using System.Threading.Tasks;
using In.ProjectEKA.HipLibrary.Patient.Model;
using In.ProjectEKA.HipService.Logger;

namespace In.ProjectEKA.HipService.DataFlow.Omod
{
    public class OmodClient
    {
        private readonly HttpClient httpClient;

        public OmodClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        private async Task GetTo(string patientId, string fromDate, string toDate, string careContextReference, 
            string careContextType, HiType hiType)
        {
            try
            {
                switch (hiType)
                {
                    case HiType.Condition:
                        var bundle = await httpClient.GetAsync(
                            $"http://192.168.33.10/openmrs/ws/rest/v1/hip/prescriptions?patientId=" +
                            patientId + "&fromDate=" + fromDate + "&toDate=" + toDate + "&careContextReference=" +
                            careContextReference +
                            "&careContextType=" + careContextType);
                        break;
                    case HiType.Observation: break;
                    default:
                        Log.Error("Hi Type Do not Match");
                        return;
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception, exception.StackTrace);
            }
        }
    }
}