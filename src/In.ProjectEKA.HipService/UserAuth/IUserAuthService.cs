using System;
using In.ProjectEKA.HipLibrary.Patient.Model;
using In.ProjectEKA.HipService.Gateway;
using In.ProjectEKA.HipService.UserAuth.Model;

namespace In.ProjectEKA.HipService.UserAuth
{
    public interface IUserAuthService
    {
        public Tuple<GatewayFetchModesRequestRepresentation,ErrorRepresentation> FetchModeResponse(
            FetchRequest fetchRequest, GatewayConfiguration gatewayConfiguration);

        public Tuple<GatewayAuthInitRequestRepresentation,ErrorRepresentation> AuthInitResponse(
            AuthInitRequest authInitRequest, GatewayConfiguration gatewayConfiguration);

        public Tuple<GatewayAuthConfirmRequestRepresentation,ErrorRepresentation> AuthConfirmResponse(
            AuthConfirmRequest authConfirmRequest);
    }
}