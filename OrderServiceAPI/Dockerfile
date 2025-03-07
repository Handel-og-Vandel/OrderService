FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5002

ENV ASPNETCORE_URLS=http://+:5002

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["OrderServiceAPI/OrderServiceAPI.csproj", "OrderServiceAPI/"]
RUN dotnet restore "OrderServiceAPI/OrderServiceAPI.csproj"
COPY . .
WORKDIR "/src/OrderServiceAPI"
RUN dotnet build "OrderServiceAPI.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "OrderServiceAPI.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderServiceAPI.dll"]
