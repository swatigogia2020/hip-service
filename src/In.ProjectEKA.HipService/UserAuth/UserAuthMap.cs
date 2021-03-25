using System;
using System.Collections.Generic;
using In.ProjectEKA.HipService.Link.Model;

namespace In.ProjectEKA.HipService.UserAuth
{
    public static class UserAuthMap{
        public static Dictionary<Guid, List<Mode>> RequestIdToAuthModes = new Dictionary<Guid, List<Mode>>();
        public static Dictionary<Guid, string> RequestIdToTransactionIdMap = new Dictionary<Guid, string>();
        public static Dictionary<Guid, string> RequestIdToAccessToken = new Dictionary<Guid, string>();
        public static Dictionary<string, string> HealthIdToTransactionId = new Dictionary<string, string>();
    }
}