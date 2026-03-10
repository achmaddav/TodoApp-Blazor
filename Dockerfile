# 1. Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy semua file project untuk restore dependencies
COPY *.sln .
COPY TodoApp.API/*.csproj ./TodoApp.API/
COPY TodoApp.Application/*.csproj ./TodoApp.Application/
COPY TodoApp.Domain/*.csproj ./TodoApp.Domain/
COPY TodoApp.Infrastructure/*.csproj ./TodoApp.Infrastructure/
COPY TodoApp.Web/*.csproj ./TodoApp.Web/
RUN dotnet restore

# Copy seluruh source code dan build
COPY . .
WORKDIR /source/TodoApp.Web
RUN dotnet publish -c Release -o /app/publish

# 2. Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
# Jalankan aplikasi (Sesuaikan jika nama DLL berbeda)
ENTRYPOINT ["dotnet", "TodoApp.Web.dll"]