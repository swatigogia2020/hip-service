namespace In.ProjectEKA.HipService.DataFlow
{
    public static class ErrorMessage
    {
        public static readonly string InternalServerError = "Internal Server Error";
        public static readonly string ContextArtefactIdNotFound = "Consent artefact not found";
        public static readonly string InvalidToken = "Token is invalid";
        public static readonly string HealthInformationNotFound = "Health information not found";
        public static readonly string LinkExpired = "Link has expired";
        public static readonly string ExpiredKeyPair = "Key material expired";
        public static readonly string InvalidHealthId = "Health Id is invalid. Must contain at least 4 letters. " +
                                                        " We only allow alphabets and numbers and" +
                                                        " do not allow special character except dot (.)";
    }
}