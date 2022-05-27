using System.Threading.Tasks;
using NUnit.Framework;

namespace Validator.Test
{
    public class SqlValidatorTests : TestBase
    {
        [Test]
        public async Task GivesExceptionIfSqlIsInvalid()
        {
            var validator = GetValidator();

            var result = await validator.TestSql("selet * from MyTable;", "test.sql");
            Assert.That(result, Is.EqualTo(new TestResult(false, "test.sql", new TestError("Incorrect syntax near 'selet'.", 1))));
        }

        [Test]
        public async Task GivesExceptionIfSqlIsInvalid_HandlesGoStatements()
        {
            var validator = GetValidator();

            var sql = @"select * from MyTable;
GO

selet * from MyTable;";

            var result = await validator.TestSql(sql, "test.sql");
            Assert.That(result, Is.EqualTo(new TestResult(false, "test.sql", new TestError("Incorrect syntax near 'selet'.", 4))));
        }

        [Test]
        public async Task GivesNoExceptionIfSqlIsValid()
        {
            var validator = GetValidator();

            var result = await validator.TestSql("select * from MyTable;", "test.sql");
            Assert.That(result, Is.EqualTo(new TestResult(true, "test.sql", null)));
        }
        
        [Test]
        public async Task GivesNoExceptionIfSqlIsValid_HandlesGoStatements()
        {
            var validator = GetValidator();

            var sql = @"select * from MyTable;
GO

select * from MyTable;";
            
            var result = await validator.TestSql(sql, "test.sql");
            Assert.That(result, Is.EqualTo(new TestResult(true, "test.sql", null)));
        }
    }
}