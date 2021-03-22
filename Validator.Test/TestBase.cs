using System.Threading.Tasks;
using NUnit.Framework;

namespace Validator.Test
{
    public abstract class TestBase
    {
        protected const string ConnectionString = "Server=localhost;Database=master;User Id=sa;Password=Passw0rd;";
        
        protected static SqlValidator GetValidator()
        {
            return new(ConnectionString);
        }
        
        [Description("Make sure a connection is actually available for the tests to run against")]
        [OneTimeSetUp]
        public static async Task BeforeAll()
        {
            var validator = GetValidator();
            await validator.TestConnection();
        }

        
    }
}