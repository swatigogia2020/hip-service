namespace In.ProjectEKA.HipService.UserAuth.Model
{
    public class FetchRequest
    {
        public string healthId { get; }
        public string purpose { get; }

        public FetchRequest(string healthId, string purpose)
        {
            this.healthId = healthId;
            this.purpose = purpose;
        }
    }
}