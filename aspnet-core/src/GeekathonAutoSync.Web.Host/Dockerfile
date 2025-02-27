FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY ["src/GeekathonAutoSync.Web.Host/GeekathonAutoSync.Web.Host.csproj", "src/GeekathonAutoSync.Web.Host/"]
COPY ["src/GeekathonAutoSync.Web.Core/GeekathonAutoSync.Web.Core.csproj", "src/GeekathonAutoSync.Web.Core/"]
COPY ["src/GeekathonAutoSync.Application/GeekathonAutoSync.Application.csproj", "src/GeekathonAutoSync.Application/"]
COPY ["src/GeekathonAutoSync.Core/GeekathonAutoSync.Core.csproj", "src/GeekathonAutoSync.Core/"]
COPY ["src/GeekathonAutoSync.EntityFrameworkCore/GeekathonAutoSync.EntityFrameworkCore.csproj", "src/GeekathonAutoSync.EntityFrameworkCore/"]
WORKDIR "/src/src/GeekathonAutoSync.Web.Host"
RUN dotnet restore 

WORKDIR /src
COPY ["src/GeekathonAutoSync.Web.Host", "src/GeekathonAutoSync.Web.Host"]
COPY ["src/GeekathonAutoSync.Web.Core", "src/GeekathonAutoSync.Web.Core"]
COPY ["src/GeekathonAutoSync.Application", "src/GeekathonAutoSync.Application"]
COPY ["src/GeekathonAutoSync.Core", "src/GeekathonAutoSync.Core"]
COPY ["src/GeekathonAutoSync.EntityFrameworkCore", "src/GeekathonAutoSync.EntityFrameworkCore"]
WORKDIR "/src/src/GeekathonAutoSync.Web.Host"
RUN dotnet publish -c Release -o /publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
EXPOSE 80
WORKDIR /app
COPY --from=build /publish .
ENTRYPOINT ["dotnet", "GeekathonAutoSync.Web.Host.dll"]
