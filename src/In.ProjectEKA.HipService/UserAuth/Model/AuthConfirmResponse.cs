namespace In.ProjectEKA.HipService.UserAuth.Model
{
    public class AuthConfirmResponse
    {
        public AuthConfirmResponse(AuthConfirmPatient patient)
        {
            this.patient = patient;
        }

        public AuthConfirmPatient patient { get; }
    }
}