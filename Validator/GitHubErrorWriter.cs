using System.Collections.Generic;
using System.Linq;

namespace Validator
{
    public class GitHubErrorWriter
    {
        private readonly IOutputWriter _output;

        public GitHubErrorWriter(IOutputWriter output)
        {
            _output = output;
        }

        public void WriteErrors(List<TestResult> results)
        {
            var errors = results.Where(r => !r.Success);

            foreach (var testResult in errors)
            {
                _output.WriteLine($"::error file={testResult.FileName},line={testResult.Error!.LineNumber}::{testResult.Error!.Message}");
            }
        }
    }
}