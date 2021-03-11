namespace In.ProjectEKA.HipService.Linkage
{
    public class AuthInitQuery
    {
        public string id { get; }
        public string purpose { get; }
        public string authMode { get; }
        public Requester requester {get; }

        public AuthInitQuery(string id, string purpose, string authMode, Requester requester ){
            this.id = id;
            this.purpose = purpose;
            this.authMode = authMode;
            this.requester = requester;
        }
    }
}