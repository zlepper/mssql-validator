#!/bin/bash

/opt/mssql/bin/sqlservr > /dev/null 2>&1 &
/app/Validator "$@"
