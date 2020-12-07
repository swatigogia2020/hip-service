using System;
using In.ProjectEKA.HipLibrary.Patient.Model;

namespace In.ProjectEKA.HipService.DataFlow
{
    using System.Threading.Tasks;
    using Optional;

    public interface ICollectHipService
    {
          Task<Option<Entries>> CollectData(TraceableDataRequest dataRequest);
    }
}