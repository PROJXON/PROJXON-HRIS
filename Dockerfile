FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["ProjxonHRIS.sln", "."]
COPY ["CloudSync/CloudSync.csproj", "CloudSync/"]
COPY ["Shared/Shared.csproj", "Shared/"]
RUN dotnet restore "CloudSync/CloudSync.csproj"
RUN dotnet restore "Shared/Shared.csproj"
COPY ["CloudSync/", "CloudSync/"]
COPY ["Shared/", "Shared/"]
WORKDIR "/src/CloudSync"
RUN dotnet build "CloudSync.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CloudSync.csproj" -c Release -o /app/publish 

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CloudSync.dll"]
