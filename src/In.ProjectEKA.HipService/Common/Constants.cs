namespace In.ProjectEKA.HipService.Common
{
    public static class Constants
    {
        public const string CURRENT_VERSION = "v0.5";
        public const string PATH_SESSIONS = CURRENT_VERSION + "/sessions";
        public const string PATH_CARE_CONTEXTS_DISCOVER = CURRENT_VERSION + "/care-contexts/discover";
        public const string PATH_CONSENTS_HIP = CURRENT_VERSION + "/consents/hip/notify";
        public const string PATH_LINKS_LINK_INIT = CURRENT_VERSION + "/links/link/init";
        public const string PATH_LINKS_LINK_CONFIRM = CURRENT_VERSION + "/links/link/confirm";
        public const string PATH_HEALTH_INFORMATION_HIP_REQUEST = CURRENT_VERSION + "/health-information/hip/request";
        public const string PATH_HEART_BEAT = CURRENT_VERSION + "/heartbeat";
        public const string PATH_READINESS = CURRENT_VERSION + "/readiness";

        public const string PATH_ON_AUTH_CONFIRM = CURRENT_VERSION + "/users/auth/on-confirm";
        public const string PATH_ON_AUTH_INIT = "/" + CURRENT_VERSION + "/users/auth/on-init";
        public const string PATH_ON_FETCH_AUTH_MODES = "/" + CURRENT_VERSION + "/users/auth/on-fetch-modes";
        public const string PATH_ON_ADD_CONTEXTS = "/" + CURRENT_VERSION + "/links/link/on-add-contexts";
        public const string PATH_ON_NOTIFY_CONTEXTS = "/" + CURRENT_VERSION + "/links/context/on-notify";
        public static readonly string DateTimeFormat = "yyyy-MM-ddTHH:mm:ssZ";
        public static readonly string PATH_ON_DISCOVER = "/" + CURRENT_VERSION + "/care-contexts/on-discover";
        public static readonly string PATH_ON_LINK_INIT = "/" + CURRENT_VERSION + "/links/link/on-init";
        public static readonly string PATH_ON_LINK_CONFIRM = "/" + CURRENT_VERSION + "/links/link/on-confirm";
        public static readonly string PATH_CONSENT_ON_NOTIFY = "/" + CURRENT_VERSION + "/consents/hip/on-notify";

        public static readonly string PATH_HEALTH_INFORMATION_ON_REQUEST = "/" + CURRENT_VERSION +
                                                                           "/health-information/hip/on-request";

        public static readonly string PATH_HEALTH_INFORMATION_NOTIFY_GATEWAY = "/" + CURRENT_VERSION +
                                                                               "/health-information/notify";

        public static readonly string PATH_AUTH_CONFIRM = "/" + CURRENT_VERSION + "/users/auth/confirm";

        public static readonly string PATH_OPENMRS_FHIR = "ws/fhir2/R4/Patient";
        public static readonly string PATH_OPENMRS_REST = "ws/rest/v1/visit";
        public static readonly string PATH_OPENMRS_HITYPE = "ws/rest/v1/hip/";
        public static readonly string PATH_PATIENT_PROFILE_OPENMRS = "ws/rest/v1/bahmnicore/patientprofile";
        public static readonly string PATH_OPENMRS_IDENTIFIER_TYPE = "ws/rest/v1/idgen/identifiertype";
        public static readonly string PATH_OPENMRS_ATTRIBUTE_TYPE =
            "/ws/rest/v1/personattributetype?q=phoneNumber&v=default";
        public static readonly string PATH_OPENMRS_EXISTING_HEALTHID = PATH_OPENMRS_HITYPE + "existingPatients/";
        public static readonly string CONFIG_KEY = "OpenMrs";

        public const string CORRELATION_ID = "CORRELATION-ID";
        public const string PATH_PATIENT_PROFILE_SHARE = "/" + CURRENT_VERSION + "/patients/profile/share";
        public const string PATH_PATIENT_PROFILE_ON_SHARE = "/" + CURRENT_VERSION + "/patients/profile/on-share";
        public const string PATH_FETCH_AUTH_MODES = "/" + CURRENT_VERSION + "/users/auth/fetch-modes";
        public const string PATH_ADD_PATIENT_CONTEXTS = "/" + CURRENT_VERSION + "/links/link/add-contexts";
        public const string PATH_NOTIFY_PATIENT_CONTEXTS = "/" + CURRENT_VERSION + "/links/context/notify";
        public const string PATH_FETCH_MODES = "/" + CURRENT_VERSION + "/hip/fetch-modes";
        public const string PATH_NEW_CARECONTEXT = "/" + CURRENT_VERSION + "/hip/new-carecontext";
        public const string PATH_ADD_CONTEXTS = "/" + CURRENT_VERSION + "/hip/add-contexts";
        public const string PATH_NOTIFY_CONTEXTS = "/" + CURRENT_VERSION + "/hip/notify";
        public const string KYC_AND_LINK = "KYC_AND_LINK";
        public const string HIP = "HIP";
        public const string PATH_AUTH_INIT = "/" + CURRENT_VERSION + "/users/auth/init";
        public const string PATH_HIP_AUTH_INIT = "/" + CURRENT_VERSION + "/hip/auth/init";
        public const string PATH_HIP_AUTH_CONFIRM = "/" + CURRENT_VERSION + "/hip/auth/confirm";
        public const string PATH_ADD_NDHM_DEMOGRAPHICS = "/" + CURRENT_VERSION + "/hip/ndhm-demographics";

        public const string PATH_HIP_SMS_NOTIFY = "/" + CURRENT_VERSION + "/hip/patients/sms/notify";
        public const string PATH_SMS_NOTIFY = "/" + CURRENT_VERSION + "/patients/sms/notify";
        public const string PATH_SMS_ON_NOTIFY = "/" + CURRENT_VERSION + "/patients/sms/on-notify";

        public const string REPORTING_SESSION = "reporting_session";
        public const string OPENMRS_SESSION_ID_COOKIE_NAME = "JSESSIONID";
        public const string WHO_AM_I = "/ws/rest/v1/bahmnicore/whoami";

        public const string MOBILE = "MOBILE";

        public const string DEEPLINK_URL = "https://link.to.health.records";
        public const string PATH_PATIENT_NOTIFY = "/" + CURRENT_VERSION + "/patients/status/notify";
        public const string PATH_PATIENT_ON_NOTIFY = "/" + CURRENT_VERSION + "/patients/status/on-notify";
        public const string PATH_PROFILE_SHARE = "/" + CURRENT_VERSION + "/patients/profile/share";
        public const string PATH_PROFILE_ON_SHARE = "/" + CURRENT_VERSION + "/patients/profile/on-share";
    }
}