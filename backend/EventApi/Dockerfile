FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY EventApi.csproj ./
RUN dotnet restore EventApi.csproj
COPY . .
RUN dotnet publish EventApi.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
RUN apt-get update && apt-get install -y --no-install-recommends netcat-traditional curl && apt-get clean && rm -rf /var/lib/apt/lists/*
ENTRYPOINT ["dotnet", "EventApi.dll"]