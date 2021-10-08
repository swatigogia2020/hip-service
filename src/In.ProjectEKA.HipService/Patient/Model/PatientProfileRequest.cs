using System.Collections.Generic;

namespace In.ProjectEKA.HipService.Patient.Model
{
    public class PatientProfileRequest
    {
        public PatientProfileRequest(OpenMrsPatient patient, List<object> relationships)
        {
            this.patient = patient;
            this.relationships = relationships;
        }
        public OpenMrsPatient patient { get; set; }
        public List<object> relationships { get; set; }
    }
}