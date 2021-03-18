namespace In.ProjectEKA.HipService.UserAuth.Model
{
    public class Validity
    {
        public string purpose { get; }
        public Requester requester { get; }

        public Validity(string purpose, Requester requester)
        {
            this.purpose = purpose;
            this.requester = requester;
        }
    }
}