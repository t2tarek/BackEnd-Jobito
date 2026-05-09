@echo off
echo Generating SQL Script for Database Setup...
dotnet ef migrations script -i -o database_setup.sql
if %errorlevel% neq 0 (
    echo.
    echo ERROR: Could not generate SQL script. 
    echo Make sure "dotnet-ef" tool is installed. 
    echo You can install it using: dotnet tool install --global dotnet-ef
) else (
    echo.
    echo SUCCESS! Script generated in "database_setup.sql".
    echo Copy the content of this file and run it in the MonsterASP T-SQL window.
)
pause
