pushd .\src\sdk\


REM Clean old packages
del .\bin\*.nupkg /s /q

REM If more than one project file delete backups

dotnet pack -c release --include-symbols  --include-source -v n

dotnet nuget push --source "privatenuget" --api-key az --interactive .\bin\release\*.nupkg

start msedge "https://dev.azure.com/darkmatter2bd/NuGet/_packaging?_a=feed&feed=privatenuget"

popd


