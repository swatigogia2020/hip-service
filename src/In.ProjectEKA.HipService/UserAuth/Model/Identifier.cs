namespace In.ProjectEKA.HipService.UserAuth.Model
{
    public class Identifier
    {
        public string type { get; }
        public string value { get; }

        public Identifier(string type, string value)
        {
            this.type = type;
            this.value = value;
        }
    }
}