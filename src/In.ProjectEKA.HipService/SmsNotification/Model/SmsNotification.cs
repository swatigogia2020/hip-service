namespace In.ProjectEKA.HipService.SmsNotification.Model
{
    public class SmsNotification
    {
        public string phoneNo { get; }
        
        public string receiverName { get; }
        
        public string careContextInfo { get; }
        
        public string deeplinkUrl { get; }
        
        public SmsNotifyHip hip { get; }

        public SmsNotification(string phoneNo, string receiverName, string careContextInfo, string deeplinkUrl,
            SmsNotifyHip hip)
        {
            this.phoneNo = phoneNo;
            this.receiverName = receiverName;
            this.careContextInfo = careContextInfo;
            this.deeplinkUrl = deeplinkUrl;
            this.hip = hip;
        }
    }
}