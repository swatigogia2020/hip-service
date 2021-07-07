namespace In.ProjectEKA.HipService.Link.Model
{
    public class AddCareContextsLink
    {
        public string accessToken { get; }
        public AddCareContextsPatient patient { get; }

        public AddCareContextsLink(string accessToken, AddCareContextsPatient addCareContextsPatient)
        {
            this.accessToken = accessToken;
            this.patient = addCareContextsPatient;
        }
    }
}