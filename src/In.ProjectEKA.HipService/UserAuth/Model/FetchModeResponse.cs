using In.ProjectEKA.HipLibrary.Patient.Model;
using In.ProjectEKA.HipService.Link.Model;

namespace In.ProjectEKA.HipService.UserAuth.Model
{
    public class FetchModeResponse
    {
        public string[] authModes { get; }

        public FetchModeResponse( string[] authModes)
        {
            this.authModes = authModes;
        }
    }
}