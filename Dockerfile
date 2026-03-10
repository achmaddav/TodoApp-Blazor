# 1. Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy file solution
COPY *.sln .

# Copy semua file project (DITAMBAH UnitTests)
COPY TodoApp.API/*.csproj ./TodoApp.API/
COPY TodoApp.Application/*.csproj ./TodoApp.Application/
COPY TodoApp.Domain/*.csproj ./TodoApp.Domain/
COPY TodoApp.Infrastructure/*.csproj ./TodoApp.Infrastructure/
COPY TodoApp.Web/*.csproj ./TodoApp.Web/
COPY TodoApp.UnitTests/*.csproj ./TodoApp.UnitTests/ 

# Restore dependencies
RUN dotnet restore

# Copy seluruh source code
COPY . .

# Build project Web
WORKDIR /source/TodoApp.Web
RUN dotnet publish -c Release -o /app/publish

# 2. Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Sesuaikan dengan nama DLL project Web kamu
ENTRYPOINT ["dotnet", "TodoApp.Web.dll"]