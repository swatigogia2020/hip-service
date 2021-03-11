namespace In.ProjectEKA.HipService.Linkage
{
    public class AuthInitRequest
    {
        public string requestId { get; }
        public string healthId { get; }
        public string authMode { get; }

        public AuthInitRequest(string requestId, string healthId, string authMode)
        {
            this.requestId = requestId;
            this.healthId = healthId;
            this.authMode = authMode;
        }
    }
}