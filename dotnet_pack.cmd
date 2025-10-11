rmdir packages /q /s
dotnet pack -c release --output packages
pause