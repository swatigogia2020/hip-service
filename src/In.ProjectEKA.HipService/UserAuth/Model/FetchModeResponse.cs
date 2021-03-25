using System.Collections.Generic;
using In.ProjectEKA.HipService.Link.Model;

namespace In.ProjectEKA.HipService.UserAuth.Model
{
    public class FetchModeResponse
    {
        public List<Mode> authModes { get; }

        public FetchModeResponse( List<Mode> authModes)
        {
            this.authModes = authModes;
        }
    }
}