FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /epcis
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /epcis
COPY ["src/FasTnT.Host/FasTnT.Host.csproj", "FasTnT.Host/"]
COPY ["src/FasTnT.Application/FasTnT.Application.csproj", "FasTnT.Application/"]
COPY ["src/FasTnT.Domain/FasTnT.Domain.csproj", "FasTnT.Domain/"]
COPY ["src/Providers/FasTnT.Sqlite/FasTnT.Sqlite.csproj", "Providers/FasTnT.Sqlite/"]
COPY ["src/Providers/FasTnT.Postgres/FasTnT.Postgres.csproj", "Providers/FasTnT.Postgres/"]
COPY ["src/Providers/FasTnT.SqlServer/FasTnT.SqlServer.csproj", "Providers/FasTnT.SqlServer/"]
RUN dotnet restore "FasTnT.Host/FasTnT.Host.csproj"

WORKDIR "/epcis/FasTnT.Host"
RUN dotnet build "FasTnT.Host.csproj" -c Release -o /epcis/build

FROM build AS publish
RUN dotnet publish "FasTnT.Host.csproj" -c Release -o /epcis/publish

FROM base AS final
WORKDIR /epcis
COPY --from=publish /epcis/publish .
ENTRYPOINT ["dotnet", "FasTnT.Host.dll"]
