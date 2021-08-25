using System.Collections.Generic;
using System.Threading.Tasks;

namespace In.ProjectEKA.HipService.OpenMrs
{
    using Hl7.Fhir.Model;
    public interface IPatientDal
    {
        Task<List<Patient>> LoadPatientsAsyncWithDemographics(string name, AdministrativeGender? gender, string yearOfBirth);
        Task<List<Patient>> LoadPatientsAsyncWithId(string id);
        Task<Patient> LoadPatientAsyncWithIdentifier(string patientIdentifier);
    }
}