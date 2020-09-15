using System.Linq;
using System.Threading.Tasks;
using In.ProjectEKA.HipLibrary.DataFlow;
using In.ProjectEKA.HipLibrary.Patient;
using In.ProjectEKA.HipLibrary.Patient.Model;
using Optional;

namespace In.ProjectEKA.HipService.DataFlow
{
    public class CollectDataflowBundle : ICollect
    {
        private IOpenMrsDataFlowRepository _openMrsDataFlowRepository;

        public CollectDataflowBundle(IOpenMrsDataFlowRepository openMrsDataFlowRepository)
        {
            _openMrsDataFlowRepository = openMrsDataFlowRepository;
        }

        public async Task<Option<Entries>> CollectData(HipLibrary.Patient.Model.DataRequest dataRequest)
        {
            var bundles = dataRequest
                .CareContexts
                .Select(async cc =>
                {
                    var medications =
                        await _openMrsDataFlowRepository.GetMedicationsForVisits(cc.PatientReference, cc.CareContextReference);
                    return new CareBundle(cc.CareContextReference, medications);
                })
                .ToList();

            var carebundles = await Task.WhenAll(bundles);

            return Option.Some(new Entries(carebundles));
        }
    }
}