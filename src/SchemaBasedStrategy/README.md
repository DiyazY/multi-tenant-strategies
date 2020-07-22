Call commands from current project:
dotnet ef --startup-project ..\multitenancy-db migrations add [migration name] --context SchemaBasedContext
dotnet ef --startup-project ..\multitenancy-db database update --context SchemaBasedContext

