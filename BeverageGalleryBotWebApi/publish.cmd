@REM windows
dotnet publish -c Release -r win-x64 -o ./publish --self-contained true

@REM Linux
dotnet publish -c Release -r linux-x64 -o ./publish --self-contained true