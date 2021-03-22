using System.Collections.Generic;
using NUnit.Framework;

namespace Validator.Test
{
    [TestFixture]
    public class GitHubErrorWriterTests
    {
        [Test]
        public void WritesErrors()
        {
            var input = new List<TestResult>
            {
                new(false, "myfile.sql", new TestError("Something went wrong", 42))
            };

            var wrapper = new OutputWrapper();
            var writer = new GitHubErrorWriter(wrapper);
            
            writer.WriteErrors(input);
            
            Assert.That(wrapper.Lines, Is.EquivalentTo(new List<string>
            {
                "::error file=myfile.sql,line=42::Something went wrong"
            }));
        }

        [Test]
        public void DoesNotWriteSuccesses()
        {
            var input = new List<TestResult>
            {
                new(true, "myfile.sql", null)
            };

            var wrapper = new OutputWrapper();
            var writer = new GitHubErrorWriter(wrapper);
            
            writer.WriteErrors(input);
            
            Assert.That(wrapper.Lines, Is.Empty);
        }

        private class OutputWrapper : IOutputWriter
        {
            public readonly List<string> Lines = new();
            
            public void WriteLine(string text)
            {
                Lines.Add(text);
            }
        }
    }
}