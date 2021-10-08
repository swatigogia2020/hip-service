namespace In.ProjectEKA.HipService.Patient.Model
{
    public class PatientAddress
    {
        public PatientAddress(string address1, string countyDistrict, string stateProvince)
        {
            this.address1 = address1;
            this.countyDistrict = countyDistrict;
            this.stateProvince = stateProvince;
        }
        public string address1 { get; set; }
        public string countyDistrict { get; set; }
        public string stateProvince { get; set; }
    }
}