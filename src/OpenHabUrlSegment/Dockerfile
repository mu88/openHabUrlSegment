FROM mcr.microsoft.com/dotnet/aspnet:7.0-bullseye-slim-arm64v8 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/OpenHabUrlSegment/OpenHabUrlSegment.csproj", "src/OpenHabUrlSegment/"]
RUN dotnet restore "src/OpenHabUrlSegment/OpenHabUrlSegment.csproj"
COPY . .
WORKDIR "/src/src/OpenHabUrlSegment"
RUN dotnet build "OpenHabUrlSegment.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OpenHabUrlSegment.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OpenHabUrlSegment.dll"]
