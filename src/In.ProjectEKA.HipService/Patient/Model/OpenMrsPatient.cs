using System;
using System.Collections.Generic;

namespace In.ProjectEKA.HipService.Patient.Model
{
    public class OpenMrsPatient
    {
        public OpenMrsPatient(Person person, List<object> identifiers)
        {
            this.person = person;
            this.identifiers = identifiers;
        }
        public Person person { get; set; }
        public List<Object> identifiers { get; set; }
    }
}