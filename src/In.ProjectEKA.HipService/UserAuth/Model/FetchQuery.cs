using In.ProjectEKA.HipService.Common;

namespace In.ProjectEKA.HipService.UserAuth.Model
{
    public class FetchQuery
    {
        public string id { get; }
        public string purpose { get; } = Constants.FETCH_MODE_PURPOSE;
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