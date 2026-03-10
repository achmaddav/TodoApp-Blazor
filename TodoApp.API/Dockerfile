FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/TodoApp.API/TodoApp.API.csproj", "src/TodoApp.API/"]
COPY ["src/TodoApp.Application/TodoApp.Application.csproj", "src/TodoApp.Application/"]
COPY ["src/TodoApp.Domain/TodoApp.Domain.csproj", "src/TodoApp.Domain/"]
COPY ["src/TodoApp.Infrastructure/TodoApp.Infrastructure.csproj", "src/TodoApp.Infrastructure/"]

RUN dotnet restore "src/TodoApp.API/TodoApp.API.csproj"

COPY . .
WORKDIR "/src/src/TodoApp.API"
RUN dotnet build "TodoApp.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TodoApp.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TodoApp.API.dll"]