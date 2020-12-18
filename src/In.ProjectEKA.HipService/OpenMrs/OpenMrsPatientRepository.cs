using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using In.ProjectEKA.HipLibrary.Patient;
using In.ProjectEKA.HipService.OpenMrs.Mappings;
using Optional;

namespace In.ProjectEKA.HipService.OpenMrs
{
    using In.ProjectEKA.HipLibrary.Patient.Model;

    public class OpenMrsPatientRepository : IPatientRepository
    {
        private readonly IPatientDal _patientDal;
        private readonly ICareContextRepository _careContextRepository;
        private readonly IPhoneNumberRepository _phoneNumberRepository;

        public OpenMrsPatientRepository(IPatientDal patientDal, ICareContextRepository careContextRepository,
            IPhoneNumberRepository phoneNumberRepository)
        {
            _patientDal = patientDal;
            _careContextRepository = careContextRepository;
            _phoneNumberRepository = phoneNumberRepository;
        }

        public async Task<Option<Patient>> PatientWithAsync(string patientIdentifier)
        {
            var fhirPatient = await _patientDal.LoadPatientAsyncWithIdentifier(patientIdentifier);
            var firstName = fhirPatient.Name[0].GivenElement.FirstOrDefault().ToString();
            var hipPatient = fhirPatient.ToHipPatient(firstName);
            var referenceNumber = hipPatient.Uuid;
            hipPatient.CareContexts = await _careContextRepository.GetCareContexts(referenceNumber);
            hipPatient.PhoneNumber = await _phoneNumberRepository.GetPhoneNumber(referenceNumber);

            return Option.Some(hipPatient);
        }

        public async Task<IQueryable<Patient>> PatientsWithVerifiedId(string name, AdministrativeGender? gender,
            string yearOfBirth, string phoneNumber)
        {
            var fhirPatients = await _patientDal.LoadPatientsAsync(name, gender, yearOfBirth);
            List<Patient> result = new List<Patient>();
            fhirPatients.ForEach( patient =>
            {
                var firstName = patient.Name[0].GivenElement.FirstOrDefault().ToString();
                var hipPatient = patient.ToHipPatient(firstName);
                var referenceNumber = hipPatient.Uuid;
                var bahmniPhoneNumber =   _phoneNumberRepository.GetPhoneNumber(referenceNumber).Result;
                if (phoneNumber == bahmniPhoneNumber)
                {
                    result.Add(hipPatient);
                }
            });

            return result.ToList().AsQueryable();
        }
    }
}