using Bogus;

namespace In.ProjectEKA.HipServiceTest.Linkage
{
    public class TestBuilder
    {
        private static Faker faker;
        internal static Faker Faker()
        {
            return faker ??= new Faker();
        }
    }
}