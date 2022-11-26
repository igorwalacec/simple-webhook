#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Webhook.Api/Webhook.Api.csproj", "Webhook.Api/"]
RUN dotnet restore "Webhook.Api/Webhook.Api.csproj"
COPY . .
WORKDIR "/src/Webhook.Api"
RUN dotnet build "Webhook.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Webhook.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Webhook.Api.dll"]