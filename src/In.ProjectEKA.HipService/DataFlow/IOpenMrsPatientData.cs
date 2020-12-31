using System;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using In.ProjectEKA.HipLibrary.Patient.Model;

namespace In.ProjectEKA.HipService.DataFlow
{
    public interface IOpenMrsPatientData
    {
        public Task<string> GetPatientData(string patientUuid, string grantedContext, string toDate, string fromDate,
            string hiType);
    }
}