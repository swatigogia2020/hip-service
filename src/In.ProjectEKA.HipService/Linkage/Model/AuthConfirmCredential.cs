namespace In.ProjectEKA.HipService.Linkage
{
    public class AuthConfirmCredential
    {
        public string authCode { get; }

        public AuthConfirmCredential(string authCode)
        {
            this.authCode = authCode;
        }
    }
}