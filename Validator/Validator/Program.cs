using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;

namespace Validator
{
    class Program
    {
        public class Options
        {
            [Option('p', "pattern", Required = true, HelpText = "The file pattern to use when looking for files")]
            public string Pattern { get; set; } = null!;

            [Option('c', "connection-string", Required = false, HelpText = "The connection string to use, in case you want to connect to a different database than by default")]
            public string ConnectionString { get; set; } = "Server=localhost;Database=master;User Id=sa;Password=Passw0rd;";
        }
        
        static async Task Main(string[] args)
        {
            await Parser.Default.ParseArguments<Options>(args)
                .WithNotParsed(errors => throw new Exception("Invalid commandline: " + string.Join("; ", errors)))
                .WithParsedAsync(async options =>
                {
                    var validator = new SqlValidator(options.ConnectionString);

                    var tokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(1d));
                    await WaitForReady(validator, tokenSource.Token);
                    
                    var dirInfo = new DirectoryInfo(".");

                    var executor = new TestExecutor(options.ConnectionString, options.Pattern);

                    var results = await executor.Validate(dirInfo);

                    var writer = new GitHubErrorWriter(new ConsoleOutputWrapper());
                    
                    writer.WriteErrors(results);

                    if (results.Any(r => !r.Success))
                    {
                        Environment.Exit(1);
                    }
                });
        }

        private static async Task WaitForReady(SqlValidator validator, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await validator.TestConnection();
                    return;
                }
                catch (Exception)
                {
                    Console.WriteLine("Database connection failed, waiting a moment and trying again");
                    await Task.Delay(1000, cancellationToken);
                }
            }
            
            cancellationToken.ThrowIfCancellationRequested();
        }

        private class ConsoleOutputWrapper : IOutputWriter
        {
            public void WriteLine(string text)
            {
                Console.WriteLine(text);
            }
        }
            
    }
}