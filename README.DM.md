# DarkMatter version

History:

2021-12-03 Brian Freeman

Changed Rest Calls to Async
Change Http to use HttpClient (Shared via constructor)
Timeout and proxy get set in HttpClient not API Client

Future:

Retry Logic should be removed and rely on HttpClient and Poly

## NuGet Private Feed Pushing
Update version in project file

Run

publish.cmd


