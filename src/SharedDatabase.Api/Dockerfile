FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/SharedDatabase.Api/SharedDatabase.Api.csproj", "src/SharedDatabase.Api/"]
COPY ["src/SharedDatabase.Infrastructure/SharedDatabase.Infrastructure.csproj", "src/SharedDatabase.Infrastructure/"]
COPY ["src/SharedDatabase.Application/SharedDatabase.Application.csproj", "src/SharedDatabase.Application/"]
COPY ["src/SharedDatabase.Domain/SharedDatabase.Domain.csproj", "src/SharedDatabase.Domain/"]
RUN dotnet restore "src/SharedDatabase.Api/SharedDatabase.Api.csproj"
COPY . .
WORKDIR "/src/src/SharedDatabase.Api"
RUN dotnet build "SharedDatabase.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SharedDatabase.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SharedDatabase.Api.dll"]
