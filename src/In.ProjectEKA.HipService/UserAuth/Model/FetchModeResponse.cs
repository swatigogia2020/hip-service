using In.ProjectEKA.HipLibrary.Patient.Model;

namespace In.ProjectEKA.HipService.UserAuth.Model
{
    public class FetchModeResponse
    {
        public string[] authModes { get; }
        public Error Error { get; }

        public FetchModeResponse(Error error, string[] authModes)
        {
            Error = error;
            this.authModes = authModes;
        }
    }
}