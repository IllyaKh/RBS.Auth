#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:6.0 AS build
WORKDIR /src
COPY ["RBS.Auth.WebApi/RBS.Auth.WebApi.csproj", "RBS.Auth.WebApi/"]
COPY ["RBS.Auth.Db/RBS.Auth.Db.csproj", "RBS.Auth.Db/"]
COPY ["RBS.Auth.Services/RBS.Auth.Services.csproj", "RBS.Auth.Services/"]
COPY ["RBS.Auth.Common/RBS.Auth.Common.csproj", "RBS.Auth.Common/"]
RUN dotnet restore "RBS.Auth.WebApi/RBS.Auth.WebApi.csproj"
COPY . .
WORKDIR "/src/RBS.Auth.WebApi"
RUN dotnet build "RBS.Auth.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RBS.Auth.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "RBS.Auth.WebApi.dll"]
CMD ASPNETCORE_URLS=http://*:$PORT dotnet RBS.Auth.WebApi.dll