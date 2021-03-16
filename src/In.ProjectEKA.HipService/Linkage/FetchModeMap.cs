using System;
using System.Collections.Generic;

namespace In.ProjectEKA.HipService.Linkage
{
    public static class FetchModeMap{
        public static Dictionary<Guid, string> RequestIdToFetchMode = new Dictionary<Guid, string>();
        public static Dictionary<Guid, string> RequestIdToAccessToken = new Dictionary<Guid, string>();
    }
}