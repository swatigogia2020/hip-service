namespace In.ProjectEKA.HipService.Link.Model
{
    public class NotificationCareContext
    {
        public string patientReference { get; }
        public string careContextReference { get; }

        public NotificationCareContext(string patientReference, string careContextReference)
        {
            this.patientReference = patientReference;
            this.careContextReference = careContextReference;
        }
    }
}