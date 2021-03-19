using In.ProjectEKA.HipService.Common;

namespace In.ProjectEKA.HipService.UserAuth.Model
{
    public class AuthInitQuery
    {
        public string id { get; }
        public string purpose { get; } = Constants.FETCH_MODE_PURPOSE;
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