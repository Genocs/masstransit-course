#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Genocs.MassTransit.Customers.WebApi/Genocs.MassTransit.Customers.WebApi.csproj", "Genocs.MassTransit.Customers.WebApi/"]
RUN dotnet restore "Genocs.MassTransit.Customers.WebApi/Genocs.MassTransit.Customers.WebApi.csproj"
COPY . .
WORKDIR "/src/Genocs.MassTransit.Customers.WebApi"
RUN dotnet build "Genocs.MassTransit.Customers.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Genocs.MassTransit.Customers.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Genocs.MassTransit.Customers.WebApi.dll"]