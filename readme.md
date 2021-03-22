# MSSQL Validator
Validates that your T-SQL files are valid T-SQL.

## Example
This will checkout your code and validate all files in the `scripts` directory, that ends in .sql, recursively.


```yaml
name: SQL

on:
  push:

jobs:
  validate-sql:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      - name: Validate SQL
        uses: zlepper/mssql-validator@v1
        with:
          pattern: 'scripts/**/*.sql'
```

## Parameters
|name|description|
|----|-----------|
|pattern|The file glob pattern to use to find files to check. Uses [DotNet.Glob](https://github.com/dazinator/DotNet.Glob) for matching, so check their documentation for more information on what patterns are supported|


## How does it work
Internally there is a tiny C# program, seen in the `Validator` folder, which connects to a sql server
instance that is running inside the same docker image, and then uses [`SET PARSEONLY ON`](https://docs.microsoft.com/en-us/sql/t-sql/statements/set-parseonly-transact-sql?view=sql-server-ver15) to "execute" the code. That way only the syntax itself is validated, and it doesn't requite a database that actually exists, or a potential slow execution of all the scripts. 

This also means, that it doesn't validate that you code will actually work, just that it is valid T-SQL, and that you at least shouldn't get a syntax error when trying to run the code.
