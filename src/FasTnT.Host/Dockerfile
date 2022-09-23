FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /epcis
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["FasTnT.Host/FasTnT.Host.csproj", "FasTnT.Host/"]
COPY ["FasTnT.Formatter.Xml/FasTnT.Formatter.Xml.csproj", "FasTnT.Formatter.Xml/"]
COPY ["FasTnT.Application/FasTnT.Application.csproj", "FasTnT.Application/"]
COPY ["FasTnT.Domain/FasTnT.Domain.csproj", "FasTnT.Domain/"]
COPY ["FasTnT.Subscriptions/FasTnT.Subscriptions.csproj", "FasTnT.Subscriptions/"]
RUN dotnet restore "FasTnT.Host/FasTnT.Host.csproj"

COPY . .
WORKDIR "/src/FasTnT.Host"
RUN dotnet build "FasTnT.Host.csproj" -c Release -o /epcis/build

FROM build AS publish
RUN dotnet publish "FasTnT.Host.csproj" -c Release -o /epcis/publish

FROM base AS final
WORKDIR /epcis
COPY --from=publish /epcis/publish .
ENTRYPOINT ["dotnet", "FasTnT.Host.dll"]
