#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS build
WORKDIR /src
COPY ["FasTnT.Host/FasTnT.Host.csproj", "FasTnT.Host/"]
RUN dotnet restore "FasTnT.Host/FasTnT.Host.csproj"
COPY . .
WORKDIR "/src/FasTnT.Host"
RUN dotnet build "FasTnT.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FasTnT.Host.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FasTnT.Host.dll"]