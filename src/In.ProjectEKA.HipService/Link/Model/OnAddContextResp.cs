namespace In.ProjectEKA.HipService.Link.Model
{
    public class OnAddContextResp
    {
        public string requestId { get; }

        OnAddContextResp(string requestId)
        {
            this.requestId = requestId;
        }
    }
}