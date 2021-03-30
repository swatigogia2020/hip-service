using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using In.ProjectEKA.HipLibrary.Patient.Model;

namespace In.ProjectEKA.HipService.DataFlow
{
    public interface IOpenMrsPatientData
    {
        public Task<List<string>> GetPatientData(string patientUuid, string careContextReference, string toDate, string fromDate,
            string hiType);
    }
}