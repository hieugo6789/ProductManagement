#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SE170311.Lab3/SE170311.Lab3.csproj", "SE170311.Lab3/"]
COPY ["SE170311.Lab3.Repo/SE170311.Lab3.Repo.csproj", "SE170311.Lab3.Repo/"]
RUN dotnet restore "./SE170311.Lab3/SE170311.Lab3.csproj"
COPY . .
WORKDIR "/src/SE170311.Lab3"
RUN dotnet build "./SE170311.Lab3.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SE170311.Lab3.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SE170311.Lab3.dll"]