using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using In.ProjectEKA.HipLibrary.Patient.Model;
using In.ProjectEKA.HipService.Logger;
using In.ProjectEKA.HipService.OpenMrs;
using In.ProjectEKA.HipService.Patient.Model;
using Newtonsoft.Json;
using Person = In.ProjectEKA.HipService.Patient.Model.Person;
using Task = System.Threading.Tasks.Task;

namespace In.ProjectEKA.HipService.Patient
{
    using static Common.Constants;

    public class PatientProfileService : IPatientProfileService
    {
        private const string Gan = "GAN";
        private const string Nat = "NAT";
        private readonly HttpClient _httpClient;
        private readonly OpenMrsConfiguration _openMrsConfiguration;

        public PatientProfileService(HttpClient httpClient, OpenMrsConfiguration openMrsConfiguration)
        {
            this._httpClient = httpClient;
            this._openMrsConfiguration = openMrsConfiguration;
        }

        public async Task SavePatient(ShareProfileRequest shareProfileRequest)
        {
            var name = shareProfileRequest.Profile.PatientDemographics.Name.Split(" ");
            var gender = shareProfileRequest.Profile.PatientDemographics.Gender;
            var address = shareProfileRequest.Profile.PatientDemographics.Address;
            var healthId = shareProfileRequest.Profile.PatientDemographics.HealthId;
            var healthNumber = shareProfileRequest.Profile.PatientDemographics.HealthIdNumber;
            var personIdentifiers = shareProfileRequest.Profile.PatientDemographics.Identifiers;

            if (!await IsExist(healthNumber) && !await IsExist(healthId))
            {
                var primaryContactUuid = await GetPrimaryContactUuid();
                var primaryContact = GetPrimaryContact(personIdentifiers);
                var identifiers = await GetIdentifiers(healthNumber, healthId);
                var patientName = GetName(name);
                var patientAddress = address != null
                    ? new PatientAddress(address.Line, address.District, address.State)
                    : new PatientAddress("", "", "");
                var dateOfBirth = GetDateOfBirth(shareProfileRequest.Profile.PatientDemographics);
                var attributeType = new AttributeType(primaryContactUuid);
                var attribute = new PatientAttribute(attributeType, primaryContact);
                var person = new Person(new List<PatientName>() {patientName},
                    new List<PatientAddress>() {patientAddress},
                    dateOfBirth, gender.ToString(), new List<PatientAttribute>() {attribute});
                var openMrsPatient = new OpenMrsPatient(person,
                    identifiers);
                var patientProfileRequest = new PatientProfileRequest(openMrsPatient, new List<object>());
                var request = new HttpRequestMessage(HttpMethod.Post,
                    _openMrsConfiguration.Url + PATH_PATIENT_PROFILE_OPENMRS);
                request.Content = new StringContent(JsonConvert.SerializeObject(patientProfileRequest),
                    Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
                var content = await response.Content.ReadAsStringAsync();
                Log.Information(content);
            }
        }

        private async Task<string> GetPrimaryContactUuid()
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                _openMrsConfiguration.Url + PATH_OPENMRS_ATTRIBUTE_TYPE);
            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            var content = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(content);
            var root = jsonDoc.RootElement;
            return root.GetProperty("results")[0].GetProperty("uuid").ToString();
        }

        private string GetPrimaryContact(List<Identifier> personIdentifiers)
        {
            foreach (var identifiers in personIdentifiers.Where(identifiers =>
                identifiers.Type.Equals(IdentifierType.MOBILE)))
            {
                return identifiers.Value;
            }
            return "";
        }

        private DateTime GetDateOfBirth(PatientDemographics profilePatientDemographics)
        {
            var dayOfBirth = profilePatientDemographics.DayOfBirth > 0 ? profilePatientDemographics.DayOfBirth : 1;
            var monthOfBirth = profilePatientDemographics.MonthOfBirth > 0
                ? profilePatientDemographics.MonthOfBirth
                : 1;
            var yearOfBirth = profilePatientDemographics.YearOfBirth;
            return new DateTime(Convert.ToInt32(yearOfBirth), Convert.ToInt32(monthOfBirth),
                Convert.ToInt32(dayOfBirth)).ToUniversalTime();
        }

        public bool IsValidRequest(ShareProfileRequest shareProfileRequest)
        {
            var profile = shareProfileRequest?.Profile;
            var demographics = profile?.PatientDemographics;
            return demographics is {HealthIdNumber: { }, HealthId: { }, Identifiers: { }, Name: { }} && Enum.IsDefined(typeof(Gender), demographics.Gender) && demographics.YearOfBirth != 0;
        }

        private async Task<bool> IsExist(string patientIdentifier)
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                _openMrsConfiguration.Url + PATH_OPENMRS_EXISTING_HEALTHID + patientIdentifier);
            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            var content = await response.Content.ReadAsStringAsync();
            return !content.Contains("error");
        }

        private async Task<List<object>> GetIdentifiers(string healthId, string phrAddress)
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                _openMrsConfiguration.Url + PATH_OPENMRS_IDENTIFIER_TYPE);
            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            var content = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(content);
            var root = jsonDoc.RootElement;

            var identifiers = new List<Object>();
            if (root.GetArrayLength() <= 0) return identifiers;
            foreach (var jsonElement in root.EnumerateArray())
            {
                var name = jsonElement.GetProperty("name").ToString();
                switch (name)
                {
                    case "Patient Identifier":
                    {
                        var ganSourceUuid = GetGanSourceUuid(jsonElement);
                        identifiers.Add(new PatientIdentifier(ganSourceUuid, Gan,
                            jsonElement.GetProperty("uuid").ToString()));
                        break;
                    }
                    case "Health ID":
                        identifiers.Add(item: new ExtraPatientIdentifier(healthId,
                            jsonElement.GetProperty("uuid").ToString()));
                        break;
                    case "PHR Address":
                        identifiers.Add(item: new ExtraPatientIdentifier(phrAddress,
                            jsonElement.GetProperty("uuid").ToString()));
                        break;
                    case "National ID":
                        identifiers.Add(new PatientIdentifier(
                            jsonElement.GetProperty("identifierSources")[0].GetProperty("uuid").ToString(), Nat,
                            jsonElement.GetProperty("uuid").ToString()));
                        break;
                }
            }
            return identifiers;
        }

        private string GetGanSourceUuid(JsonElement root)
        {
            var identifierSources =
                root.GetProperty("identifierSources").EnumerateArray();
            return (from sources in identifierSources where sources.GetProperty("prefix").ToString().Equals("GAN") select sources.GetProperty("uuid").ToString()).FirstOrDefault();
        }

        private PatientName GetName(string[] name)
        {
            return name.Length switch
            {
                3 => new PatientName(name[0], name[1], name[2]),
                2 => new PatientName(name[0], null, name[1]),
                _ => new PatientName(name[0], null, null)
            };
        }
    }
}