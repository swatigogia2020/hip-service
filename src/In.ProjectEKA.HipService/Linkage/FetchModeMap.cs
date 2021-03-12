using System;
using System.Collections.Generic;

namespace In.ProjectEKA.HipService.Linkage
{
    public class FetchModeMap{
        public static Dictionary<Guid, string> requestIdToFetchMode = new Dictionary<Guid, string>();
        public static Dictionary<Guid, string> requestIdToAccessToken = new Dictionary<Guid, string>();
        public static Dictionary<Guid, string> requestIdToTransactionIdMap = new Dictionary<Guid, string>();
    }
}