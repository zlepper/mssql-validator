using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Validator.Test
{
    [TestFixture]
    public class TestExecutorTests : TestBase
    {
        [Test]
        public async Task ValidatesCorrectly()
        {
            var executor = new TestExecutor(ConnectionString, "TestFiles/*.sql");

            var result = await executor.Validate(new DirectoryInfo("."));

            Assert.That(result, Is.EquivalentTo(new List<TestResult>
            {
                new(true, "TestFiles/valid.sql", null),
                new(false, "TestFiles/invalid.sql", new TestError("Incorrect syntax near 'insrt'.", 1))
            }));
        }
    }
}