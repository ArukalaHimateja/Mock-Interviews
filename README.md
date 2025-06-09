# InterviewSim

After adding EF Core migrations, update the database with:

```bash
dotnet ef migrations add Init -p Domain/InterviewSim.Core/InterviewSim.Core.csproj -s Api/InterviewSim.Api/InterviewSim.Api.csproj

dotnet ef database update -p Domain/InterviewSim.Core/InterviewSim.Core.csproj -s Api/InterviewSim.Api/InterviewSim.Api.csproj
```

These commands create the initial schema and apply it to the configured database.
