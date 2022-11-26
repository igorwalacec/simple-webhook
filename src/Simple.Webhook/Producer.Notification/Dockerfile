#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Producer.Notification/Producer.Notification.csproj", "Producer.Notification/"]
RUN dotnet restore "Producer.Notification/Producer.Notification.csproj"
COPY . .
WORKDIR "/src/Producer.Notification"
RUN dotnet build "Producer.Notification.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Producer.Notification.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Producer.Notification.dll"]