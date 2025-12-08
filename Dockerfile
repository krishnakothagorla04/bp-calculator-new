# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution file
COPY *.sln ./

# Copy all project files (assuming main project is in root)
COPY *.csproj ./
COPY BP.Tests/*.csproj ./BP.Tests/

# Restore dependencies
RUN dotnet restore ./bp-calculator.sln

# Copy all source code
COPY . .

# Build the main project (assuming .csproj at root)
RUN dotnet build ./bp-calculator.sln -c Release --no-restore

# Run tests
RUN dotnet test ./BP.Tests/BP.Tests.csproj --no-build --verbosity normal

# Publish the app
RUN dotnet publish ./bp-calculator.sln -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app/bp-calculator
COPY --from=build /app/publish .

# Expose port
EXPOSE 8080
ENV ASPNETCORE_URLS=http://*:8080

# Run the app (replace MainProject.dll with your actual DLL name)
ENTRYPOINT ["dotnet", "BPCalculator.dll"]
