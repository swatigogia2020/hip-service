using System;
using System.Collections.Generic;

namespace In.ProjectEKA.HipService.UserAuth
{
    public static class UserAuthMap{
        public static Dictionary<Guid, string> RequestIdToFetchMode = new Dictionary<Guid, string>();
        public static Dictionary<Guid, string> RequestIdToTransactionIdMap = new Dictionary<Guid, string>();
        public static Dictionary<Guid, string> RequestIdToAccessToken = new Dictionary<Guid, string>();
    }
}