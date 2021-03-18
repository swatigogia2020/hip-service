using System;
using System.Collections.Generic;

namespace In.ProjectEKA.HipService.Linkage
{
    public class LinkageMap{
        public static Dictionary<string, string> RequestIdToFetchMode = new Dictionary<string, string>();
        public static Dictionary<string, string> RequestIdToAccessToken = new Dictionary<string, string>();
        public static Dictionary<string, string> RequestIdToTransactionIdMap = new Dictionary<string, string>();
    }
}