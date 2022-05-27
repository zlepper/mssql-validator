using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Validator
{
    public class SqlValidator
    {
        private readonly string _connectionString;

        public SqlValidator(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task TestConnection()
        {
            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand("select 1;", conn);
            var result = await cmd.ExecuteScalarAsync();
            if (result is not int value)
            {
                throw new Exception("Did not get an int back from database");
            }
            if (value != 1)
            {
                throw new Exception("Test command failed. Did not get '1' back");
            }
        }

        public async Task<TestResult> TestSql(string sql, string filename)
        {
            var actualQuery = "SET PARSEONLY ON;\n" + Regex.Replace(sql, "\nGO(\n|$)", "\n\n");
            
            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand(actualQuery, conn);
            try
            {
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException e)
            {
                var lineNumber = e.LineNumber - 1; // We add the first line, so "remove" it again.
                var message = e.Message;

                return new TestResult(false, filename, new TestError(message, lineNumber));
            }

            return new TestResult(true, filename, null);
        }
    }
}