using In.ProjectEKA.HipService.Gateway;

namespace In.ProjectEKA.HipService.Linkage
{
    public interface IFetchModeService
    {
        public GatewayFetchModesRequestRepresentation FetchModeResponse(
            FetchRequest fetchRequest, GatewayConfiguration gatewayConfiguration);
    }
}