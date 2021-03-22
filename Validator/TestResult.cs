namespace Validator
{
    public record TestResult(bool Success, string FileName, TestError? Error);
}