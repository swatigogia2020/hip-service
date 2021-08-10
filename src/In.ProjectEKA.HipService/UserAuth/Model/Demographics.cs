using System.Collections.Generic;
using Hl7.Fhir.Model;

namespace In.ProjectEKA.HipService.UserAuth.Model
{
    public class Demographics
    {
        public string name { get; }
        public string gender { get; }
        public string dateOfBirth { get; }
    
        public Identifier identifer { get; }

        public Demographics(string name, string gender, string dateOfBirth, Identifier identifer)
        {
            this.name = name;
            this.gender = gender;
            this.dateOfBirth = dateOfBirth;
            this.identifer = identifer;
        }
    }
}