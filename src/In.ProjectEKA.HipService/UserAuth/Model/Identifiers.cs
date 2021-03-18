namespace In.ProjectEKA.HipService.UserAuth.Model
{
    public class Identifiers
    {
        public string type { get; }
        public string value { get; }

        public Identifiers(string type, string value)
        {
            this.type = type;
            this.value = value;
        }
    }
}