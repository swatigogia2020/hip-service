namespace In.ProjectEKA.HipService.Linkage
{
    public class AuthConfirmPatient
    {
        public string id { get; }
        public string name { get; }
        public string gender { get; }
        public string yearOfBirth { get; }
        public AuthConfirmAddress address { get; }
        
        public Identifiers identifiers { get; }

        public AuthConfirmPatient(string id, string name, string gender, string yearOfBirth, AuthConfirmAddress address,  Identifiers identifiers )
        {
            this.id = id;
            this.name = name;
            this.gender = gender;
            this.yearOfBirth = yearOfBirth;
            this.address = address;
            this.identifiers = identifiers;
        }
    }
}