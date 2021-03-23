using static In.ProjectEKA.HipService.Common.Constants;

namespace In.ProjectEKA.HipService.UserAuth.Model
{
    public class FetchQuery
    {
        public string id { get; }
        public string purpose { get; } = KYC_AND_LINK;
        public Requester requester { get; }

        public FetchQuery(string id, Requester requester)
        {
            this.id = id;
            this.requester = requester;
        }

        public FetchQuery(string id, string purpose, Requester requester)
        {
            this.id = id;
            this.purpose = purpose;
            this.requester = requester;
        }
    }
}