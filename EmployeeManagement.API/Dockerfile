# EmployeeManagement.API/Dockerfile

# ------------- Build Stage -------------
    FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
    WORKDIR /src
    
    # Copy the solution file from the repository root.
    COPY EmployeeManagement.sln ./
    
    # Copy the backend project file(s).
    COPY EmployeeManagement.API/*.csproj EmployeeManagement.API/
    
    COPY EmployeeManagement.Domain/EmployeeManagement.Domain.csproj EmployeeManagement.Domain/
    COPY EmployeeManagement.Application/EmployeeManagement.Application.csproj EmployeeManagement.Application/
    COPY EmployeeManagement.Infrastructure/EmployeeManagement.Infrastructure.csproj EmployeeManagement.Infrastructure/
    # If you have tests, copy them too:
    COPY EmployeeManagement.Tests/EmployeeManagement.Tests.csproj EmployeeManagement.Tests/
    
    # Restore NuGet packages using the solution file.
    RUN dotnet restore
    
    # Copy the entire repository (all source files).
    COPY . .
    
    # Build and publish the API project in Release mode.
    RUN dotnet publish EmployeeManagement.API/EmployeeManagement.API.csproj -c Release -o /app/publish
    
    # ------------- Runtime Stage -------------
    FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
    WORKDIR /app
    COPY --from=build /app/publish .
    
    EXPOSE 80
    ENTRYPOINT ["dotnet", "EmployeeManagement.API.dll"]
    