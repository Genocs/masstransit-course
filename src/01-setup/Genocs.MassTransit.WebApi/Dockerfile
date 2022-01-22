#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Genocs.MassTransit.WebApi/Genocs.MassTransit.WebApi.csproj", "Genocs.MassTransit.WebApi/"]
RUN dotnet restore "Genocs.MassTransit.WebApi/Genocs.MassTransit.WebApi.csproj"
COPY . .
WORKDIR "/src/Genocs.MassTransit.WebApi"
RUN dotnet build "Genocs.MassTransit.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Genocs.MassTransit.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Genocs.MassTransit.WebApi.dll"]