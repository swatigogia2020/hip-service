namespace In.ProjectEKA.HipService.SmsNotification.Model
{
    public class SmsNotifyHip
    {
        public string name { get; }
        
        public string id { get; }

        public SmsNotifyHip(string name, string id)
        {
            this.name = name;
            this.id = id;
        }
    }
}