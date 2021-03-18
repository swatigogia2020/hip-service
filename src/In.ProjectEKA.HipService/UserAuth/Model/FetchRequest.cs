namespace In.ProjectEKA.HipService.UserAuth.Model
{
    public class FetchRequest
    {
        public string healthId { get; }

        public FetchRequest(string healthId)
        {
            this.healthId = healthId;
        }
    }
}