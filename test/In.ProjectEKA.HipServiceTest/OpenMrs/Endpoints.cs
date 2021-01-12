namespace In.ProjectEKA.HipServiceTest.OpenMrs
{
    public static class Endpoints
    {
        public static class OpenMrs
        {
            public const string OnProgramEnrollmentPath = "ws/rest/v1/bahmniprogramenrollment";
            public const string OnVisitPath = "ws/rest/v1/visit";
            public const string OnPatientPath = "ws/rest/v1/patient";
            public const string OnCareContextPath = "ws/rest/v1/hip/careContext";
        }

        public static class Fhir
        {
            public const string OnPatientPath = "ws/fhir2/R4/Patient";
        }
    }
}