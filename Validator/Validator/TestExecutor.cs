using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DotNet.Globbing;

namespace Validator
{
    public class TestExecutor
    {
        private readonly string _connectionString;
        private readonly Glob _glob;

        public TestExecutor(string connectionString, string pattern)
        {
            _connectionString = connectionString;
            _glob = Glob.Parse(pattern);
        }

        private List<FileInfo> FindFiles(DirectoryInfo rootDir)
        {
            return rootDir
                .EnumerateFiles("*.*", SearchOption.AllDirectories)
                .Where(file => _glob.IsMatch(GetFileNameFromRoot(rootDir, file)))
                .ToList();
        }

        private string GetFileNameFromRoot(DirectoryInfo rootDir, FileInfo file)
        {
            var dirName = rootDir.FullName;
            var fileName = file.FullName;

            return fileName.Substring(dirName.Length + 1).Replace("\\", "/");
        }
        
        public async Task<List<TestResult>> Validate(DirectoryInfo rootDir)
        {
            var filesToTest = FindFiles(rootDir);

            if (filesToTest.Count == 0)
            {
                throw new Exception("Found no files to test. Is the file pattern correct?");
            }

            var validator = new SqlValidator(_connectionString);

            var results = new List<TestResult>();
            
            foreach (var file in filesToTest)
            {
                var sql = await File.ReadAllTextAsync(file.FullName);

                var result = await validator.TestSql(sql, GetFileNameFromRoot(rootDir, file));
                
                results.Add(result);
            }

            return results;
        }
    }
}