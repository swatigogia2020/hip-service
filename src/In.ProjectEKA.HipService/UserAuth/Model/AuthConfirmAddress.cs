namespace In.ProjectEKA.HipService.UserAuth.Model
{
    public class AuthConfirmAddress
    {
        public string line { get; }
        public string district { get; }
        public string state { get; }
        public string pincode { get; }

        public AuthConfirmAddress(string line, string district, string state, string pincode)
        {
            this.line = line;
            this.district = district;
            this.state = state;
            this.pincode = pincode;
        }
    }
}