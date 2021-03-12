using System;

namespace In.ProjectEKA.HipService.Linkage
{
    public class OnConfirmAuth
    {
        public string accessToken { get; }
        public Validity validity { get; }
        public DateTime expiry { get; }
        public string limit { get; }

        public OnConfirmAuth(string accessToken, Validity validity, DateTime expiry, string limit)
        {
            this.accessToken = accessToken;
            this.validity = validity;
            this.expiry = expiry;
            this.limit = limit;
        }
    }
}