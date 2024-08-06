# Use the official .NET SDK image as the build environment
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy the csproj and restore as distinct layers
COPY *.sln .
COPY TeamsBot/*.csproj ./TeamsBot/
RUN dotnet restore

# Copy everything else and build the release
COPY TeamsBot/. ./TeamsBot/
WORKDIR /source/TeamsBot
RUN dotnet publish -c Release -o /app

# Use the official ASP.NET Core runtime as the runtime environment
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app ./

# Expose the port the app runs on
EXPOSE 80
EXPOSE 443

# Entry point for the application
ENTRYPOINT ["dotnet", "TeamsBot.dll"]