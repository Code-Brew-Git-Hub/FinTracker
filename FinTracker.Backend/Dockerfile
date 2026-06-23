FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY FinTracker.Backend.slnx .
COPY FinTracker.API/ FinTracker.API/
COPY FinTracker.Data/ FinTracker.Data/
COPY FinTracker.Domain/ FinTracker.Domain/
COPY FinTracker.Parser/ FinTracker.Parser/

RUN dotnet publish FinTracker.API/FinTracker.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

HEALTHCHECK --interval=5s --timeout=5s --retries=12 --start-period=60s \
  CMD curl -f http://localhost:8080/swagger || exit 1

ENTRYPOINT ["dotnet", "FinTracker.API.dll"]
