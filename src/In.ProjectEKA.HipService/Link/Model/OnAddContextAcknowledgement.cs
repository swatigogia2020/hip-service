namespace In.ProjectEKA.HipService.Link.Model
{
    public class OnAddContextAcknowledgement
    {
        public string status { get; }

        OnAddContextAcknowledgement(string status)
        {
            this.status = status;
        }
    }
}