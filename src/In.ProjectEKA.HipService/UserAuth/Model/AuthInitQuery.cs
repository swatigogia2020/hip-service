using static In.ProjectEKA.HipService.Common.Constants;

namespace In.ProjectEKA.HipService.UserAuth.Model
{
    public class AuthInitQuery
    {
        public string id { get; }
        public string purpose { get; } = KYC_AND_LINK;
        public string authMode { get; }
        public Requester requester { get; }

        public AuthInitQuery(string id, string authMode, Requester requester)
        {
            this.id = id;
            this.authMode = authMode;
            this.requester = requester;
        }

        public AuthInitQuery(string id, string purpose, string authMode, Requester requester)
        {
            this.id = id;
            this.purpose = purpose;
            this.authMode = authMode;
            this.requester = requester;
        }
    }
}