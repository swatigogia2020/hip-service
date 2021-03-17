using In.ProjectEKA.HipService.Gateway;

namespace In.ProjectEKA.HipService.Linkage
{
    public interface IAuthInitService
    {
        public GatewayAuthInitRequestRepresentation AuthInitResponse(
            AuthInitRequest authInitRequest, GatewayConfiguration gatewayConfiguration);
    }
}