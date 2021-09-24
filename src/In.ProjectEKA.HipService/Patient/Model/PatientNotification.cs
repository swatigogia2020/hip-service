namespace In.ProjectEKA.HipService.Patient.Model
{
    public class PatientNotification
    {
        public Action status { get; }
        public HipNotifyPatient patient { get; }

        public PatientNotification(Action status, HipNotifyPatient patient)
        {
            this.status = status;
            this.patient = patient;
        }
    }
}