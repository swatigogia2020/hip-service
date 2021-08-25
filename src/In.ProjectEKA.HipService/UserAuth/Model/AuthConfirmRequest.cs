namespace In.ProjectEKA.HipService.UserAuth.Model
{
    public class AuthConfirmRequest
    {
        public string authCode { get; }
        public string healthId { get; }
        
        public Demographics Demographic { get; }
        public AuthConfirmRequest(string authCode, string healthId, Demographics demographic)
        {
            this.authCode = authCode;
            this.healthId = healthId;
            Demographic = demographic;
        }
    }
}