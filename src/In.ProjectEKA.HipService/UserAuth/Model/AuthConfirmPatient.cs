using System.Collections.Generic;

namespace In.ProjectEKA.HipService.UserAuth.Model
{
    public class AuthConfirmPatient
    {
        public string id { get; }
        public string name { get; }
        public string gender { get; }
        public int yearOfBirth { get; }
        public int? monthOfBirth { get; }
        public int? dayOfBirth {get; }
        public AuthConfirmAddress address { get; }
        public List<Identifier> identifiers { get; }

        public AuthConfirmPatient(string id, string name, string gender, int yearOfBirth, int? monthOfBirth, int? dayOfBirth, AuthConfirmAddress address,
            List<Identifier> identifiers)
        {
            this.id = id;
            this.name = name;
            this.gender = gender;
            this.yearOfBirth = yearOfBirth;
            this.monthOfBirth = monthOfBirth ?? 1;
            this.dayOfBirth = dayOfBirth ?? 1;
            this.address = address;
            this.identifiers = identifiers;
        }
    }
}