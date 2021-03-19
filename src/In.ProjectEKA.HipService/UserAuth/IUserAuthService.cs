using In.ProjectEKA.HipService.Gateway;
using In.ProjectEKA.HipService.UserAuth.Model;

namespace In.ProjectEKA.HipService.UserAuth
{
    public interface IUserAuthService
    {
        public GatewayFetchModesRequestRepresentation FetchModeResponse(
            FetchRequest fetchRequest, GatewayConfiguration gatewayConfiguration);

        public GatewayAuthInitRequestRepresentation AuthInitResponse(
            AuthInitRequest authInitRequest, GatewayConfiguration gatewayConfiguration);

        public GatewayAuthConfirmRequestRepresentation AuthConfirmResponse(
            AuthConfirmRequest authConfirmRequest);
    }
}