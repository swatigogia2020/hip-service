namespace In.ProjectEKA.HipService.Linkage
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