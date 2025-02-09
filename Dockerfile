# ---------------------
# Build Stage
# ---------------------
    FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
    WORKDIR /src
    
    # Copy the solution file and all project files
    COPY EmployeeManagement.sln ./
    COPY EmployeeManagement.API/EmployeeManagement.API.csproj EmployeeManagement.API/
    COPY EmployeeManagement.Domain/EmployeeManagement.Domain.csproj EmployeeManagement.Domain/
    COPY EmployeeManagement.Application/EmployeeManagement.Application.csproj EmployeeManagement.Application/
    COPY EmployeeManagement.Infrastructure/EmployeeManagement.Infrastructure.csproj EmployeeManagement.Infrastructure/
    # If you have tests, copy them too:
    COPY EmployeeManagement.Tests/EmployeeManagement.Tests.csproj EmployeeManagement.Tests/
    
    # Restore dependencies for the solution
    RUN dotnet restore
    
    # Copy the rest of the source code
    COPY . .
    
    # Build and publish the API project
    RUN dotnet publish EmployeeManagement.API/EmployeeManagement.API.csproj -c Release -o /app/publish
    
    # ---------------------
    # Runtime Stage
    # ---------------------
    FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
    WORKDIR /app
    COPY --from=build /app/publish .
    ENTRYPOINT ["dotnet", "EmployeeManagement.API.dll"]
    