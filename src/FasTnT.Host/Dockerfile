FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /epcis
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/FasTnT.Host/FasTnT.Host.csproj", "FasTnT.Host/"]
COPY ["src/FasTnT.Application/FasTnT.Application.csproj", "FasTnT.Application/"]
COPY ["src/FasTnT.Application.EfCore/FasTnT.Application.EfCore.csproj", "FasTnT.Application.EfCore/"]
COPY ["src/FasTnT.Domain/FasTnT.Domain.csproj", "FasTnT.Domain/"]
COPY ["src/FasTnT.Features.v1_2/FasTnT.Features.v1_2.csproj", "FasTnT.Features.v1_2/"]
COPY ["src/FasTnT.Features.v2_0/FasTnT.Features.v2_0.csproj", "FasTnT.Features.v2_0/"]
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
