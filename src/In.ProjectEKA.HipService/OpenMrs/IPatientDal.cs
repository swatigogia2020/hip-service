using System.Collections.Generic;
using System.Threading.Tasks;

namespace In.ProjectEKA.HipService.OpenMrs
{
    using Hl7.Fhir.Model;
    public interface IPatientDal
    {
        Task<List<Patient>> LoadPatientsAsync(string name, AdministrativeGender? gender, string yearOfBirth);
        Task<Patient> LoadPatientAsync(string id);
        Task<Patient> LoadPatientAsyncWithIndentifier(string patientIdentifier);
    }
}