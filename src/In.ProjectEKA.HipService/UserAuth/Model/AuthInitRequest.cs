namespace In.ProjectEKA.HipService.UserAuth.Model
{
    public class AuthInitRequest
    {
        public string healthId { get; }
        public string authMode { get; }

        public string purpose { get; }

        public AuthInitRequest(string healthId, string authMode, string purpose)
        {
            this.purpose = purpose;
            this.healthId = healthId;
            this.authMode = authMode;
        }
    }
}