using System;
using System.Collections.Generic;

namespace In.ProjectEKA.HipService.Patient.Model
{
    public class Person
    {
        public Person(List<PatientName> names, List<PatientAddress> addresses, DateTime birthdate, string gender, List<PatientAttribute> attributes)
        {
            this.names = names;
            this.addresses = addresses;
            this.birthdate = birthdate;
            this.gender = gender;
            this.attributes = attributes;
        }
        public List<PatientName> names { get; set; }
        public List<PatientAddress> addresses { get; set; }
        public DateTime birthdate { get; set; }
        public string gender { get; set; }
        public List<PatientAttribute> attributes { get; set; }
    }
}