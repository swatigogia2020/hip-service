namespace In.ProjectEKA.HipService.UserAuth.Model
{
    public class AuthConfirmCredential
    {
        public string authCode { get; }
        public Demographics Demographics { get; }

        public AuthConfirmCredential(string authCode, Demographics demographics)
        {
            this.authCode = authCode;
            Demographics = demographics;
        }
    }
}