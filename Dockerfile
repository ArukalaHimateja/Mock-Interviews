# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY *.sln ./
COPY Api/InterviewSim.Api/InterviewSim.Api.csproj Api/InterviewSim.Api/
COPY Domain/InterviewSim.Core/InterviewSim.Core.csproj Domain/InterviewSim.Core/
COPY Client/InterviewSim.Client/InterviewSim.Client.csproj Client/InterviewSim.Client/
RUN dotnet restore
COPY . .
RUN dotnet publish Api/InterviewSim.Api/InterviewSim.Api.csproj -c Release -o /app

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app .
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s CMD curl -f http://localhost/api/health/ready || exit 1
ENTRYPOINT ["dotnet", "InterviewSim.Api.dll"]
