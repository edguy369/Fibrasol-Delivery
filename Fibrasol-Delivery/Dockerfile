FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
RUN apk add --no-cache tzdata
WORKDIR /app

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS publish
WORKDIR /src
COPY ["Fibrasol-Delivery/Fibrasol-Delivery.csproj", "Fibrasol-Delivery/"]
RUN dotnet restore "Fibrasol-Delivery/Fibrasol-Delivery.csproj" --runtime linux-musl-x64
COPY . .

RUN dotnet publish "Fibrasol-Delivery/Fibrasol-Delivery.csproj" -c Release -o /app/publish \
    --no-restore \
    --runtime linux-musl-x64 \
    --self-contained true \
    /p:IncludeAllContentForSelfExtract=true \
    /p:PublishTrimmed=true \
    /p:PublishSingleFile=true


FROM base AS final
WORKDIR /app
ENV ASPNETCORE_URLS http://+:5002
COPY --from=publish /app/publish .
ENTRYPOINT ["./Fibrasol-Delivery"]