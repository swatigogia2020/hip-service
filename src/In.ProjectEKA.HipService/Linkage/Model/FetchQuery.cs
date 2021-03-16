namespace In.ProjectEKA.HipService.Linkage
{
    public class FetchQuery{
    public string id { get; }

    public string purpose { get; }

    public Requester requester {get; }

    public FetchQuery(string id, string purpose, Requester requester ){
        this.id = id;
        this.purpose = purpose;
        this.requester = requester;
        
    }

}
}