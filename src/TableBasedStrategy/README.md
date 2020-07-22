Call commands from current projects:
dotnet ef --startup-project ..\multitenancy-db migrations add [migration name] --context TableBasedContext
dotnet ef --startup-project ..\multitenancy-db database update --context TableBasedContext

