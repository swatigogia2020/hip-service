namespace In.ProjectEKA.HipService.SmsNotification.Model
{
    public class SmsNotificationAcknowledgement
    {
        public SmsNotificationAcknowledgement(string status)
        {
            Status = status;
        }

        public string Status { get; }
    }
}