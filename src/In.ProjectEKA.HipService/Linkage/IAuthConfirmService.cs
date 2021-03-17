namespace In.ProjectEKA.HipService.Linkage
{
    public interface IAuthConfirmService
    {
        public GatewayAuthConfirmRequestRepresentation AuthConfirmResponse(
            AuthConfirmRequest authConfirmRequest);
    }
}