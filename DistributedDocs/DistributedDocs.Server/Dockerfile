#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["DistributedDocs.Server/DistributedDocs.Server.csproj", "DistributedDocs.Server/"]
COPY ["DistributedDocs.Client/DistributedDocs.Client.csproj", "DistributedDocs.Client/"]
COPY ["DistributedDocs.DocumentChanges/DistributedDocs.DocumentChanges.csproj", "DistributedDocs.DocumentChanges/"]
COPY ["DistributedDocs.VersionHistory/DistributedDocs.VersionHistory.csproj", "DistributedDocs.VersionHistory/"]
COPY ["DistributedDocs.FileSystem/DistributedDocs.FileSystem.csproj", "DistributedDocs.FileSystem/"]
COPY ["DistributedDocs.Server.ClientModels/DistributedDocs.Server.ClientModels.csproj", "DistributedDocs.Server.ClientModels/"]
COPY ["DistributedDocs.Utils/DistributedDocs.Utils.csproj", "DistributedDocs.Utils/"]
RUN dotnet restore "DistributedDocs.Server/DistributedDocs.Server.csproj"
COPY . .
WORKDIR "/src/DistributedDocs.Server"
RUN dotnet build "DistributedDocs.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DistributedDocs.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DistributedDocs.Server.dll"]