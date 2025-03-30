FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SafeFileUploaderWeb.Web/SafeFileUploaderWeb.Web.csproj", "SafeFileUploaderWeb.Web/"]
COPY ["../SafeFileUploaderWeb.Core/SafeFileUploaderWeb.Core.csproj", "SafeFileUploaderWeb.Core/"]
RUN dotnet restore "SafeFileUploaderWeb.Web/SafeFileUploaderWeb.Web.csproj"
COPY . .
WORKDIR "/src/SafeFileUploaderWeb.Web"
RUN dotnet build "SafeFileUploaderWeb.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SafeFileUploaderWeb.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SafeFileUploaderWeb.Web.dll"]
