namespace In.ProjectEKA.HipService.Linkage
{
    public class AuthInitRequest
    {
        public string healthId { get; }
        public string authMode { get; }

        public AuthInitRequest( string healthId, string authMode)
        {
            this.healthId = healthId;
            this.authMode = authMode;
        }
    }
}