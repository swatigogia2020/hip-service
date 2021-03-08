namespace In.ProjectEKA.HipService.Linkage
{
    using System.Threading.Tasks;
    using Database;
    using Microsoft.EntityFrameworkCore;
    using Model;
    using System.Object;

public class GatewayFetchModesRequestRepresentation{
    public string requestId { get; }

    public Datetime timestamp { get; }

    public FetchQuery query { get; }

    public GatewayFetchModesRequestRepresentation(string requestId ,Datetime timestamp, FetchQuery query ){
        this.requestId = requestId;
        this.timestamp = timestamp;
        this.query = query;

    }




}
}