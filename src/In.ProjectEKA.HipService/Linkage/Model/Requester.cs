namespace In.ProjectEKA.HipService.Linkage
{
    using System.Threading.Tasks;
    using Database;
    using Microsoft.EntityFrameworkCore;
    using Model;

public class Requester{
    public string id { get; }
    public string type { get; }

    public Requester(string id, string type){
        this.id = id;
        this.type = type;
    }
    
}
}