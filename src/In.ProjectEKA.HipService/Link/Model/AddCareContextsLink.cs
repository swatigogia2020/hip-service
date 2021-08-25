namespace In.ProjectEKA.HipService.Link.Model
{
    public class AddCareContextsLink
    {
        public string AccessToken { get; }
        public AddCareContextsPatient Patient { get; }

        public AddCareContextsLink(string accessToken, AddCareContextsPatient addCareContextsPatient)
        {
            AccessToken = accessToken;
            Patient = addCareContextsPatient;
        }
    }   
}