namespace In.ProjectEKA.HipService.SmsNotification.Model
{
    public class SmsNotifyRequest
    {
        public string phoneNo { get; }
        
        public string receiverName { get; }
        
        public string careContextInfo { get; }

        public SmsNotifyRequest(string phoneNo, string receiverName, string careContextInfo)
        {
            this.phoneNo = phoneNo;
            this.receiverName = receiverName;
            this.careContextInfo = careContextInfo;
        }
    }
}