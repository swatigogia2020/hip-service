﻿using System.Threading.Tasks;
using Hl7.Fhir.Model;

namespace In.ProjectEKA.HipLibrary.DataFlow
{
    public interface IOpenMrsDataFlowRepository
    {
        Task<Bundle> GetMedicationsForVisits(string patientId, string linkedCareContextVisitType);
    }
}