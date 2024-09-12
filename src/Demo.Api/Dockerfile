FROM mcr.microsoft.com/dotnet/aspnet:8.0.8-alpine3.20-amd64 AS base-env
# Preparing the runtime environment

WORKDIR /app
EXPOSE 8080


HEALTHCHECK --interval=10s \
            --timeout=5s \
            --start-period=3s \
            --retries=5 \
    CMD wget --quiet --tries=1 --spider http://localhost:8080/healthz/ready || exit 1


# Install the required dependencies to handle the internationalization
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    LC_ALL=en_US.UTF-8 \
    LANG=en_US.UTF-8

RUN apk add --no-cache \
            icu-libs \
            icu-data-full \
            tzdata

RUN apk upgrade musl

USER app

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://*:8080
ENV ASPNETCORE_HTTP_PORTS=
ENV ASPNETCORE_HTTPS_PORTS=



FROM mcr.microsoft.com/dotnet/sdk:8.0.401 AS build-env

WORKDIR /src

# Copy just the project files and restore the nuget packages
COPY ./src/Demo.Api.Users/*.csproj ./Demo.Api.Users/

# Restore nuget packages
RUN dotnet restore ./Demo.Api.Users/*.csproj --runtime linux-musl-x64


# Copy all the source code and build
COPY ./src/Demo.Api.Users/ ./Demo.Api.Users/


# Build and publish the application. Used the "--no-restore" and "--no-build" to benefit the layer caches
RUN dotnet build -c Release ./Demo.Api.Users/*.csproj --runtime linux-musl-x64

# We can use `--runtime linux-musl-x64` or `-r $(dotnet --info | grep RID | cut -b 7- | tr -d ' ')` to get the runtime identifier
RUN dotnet publish ./Demo.Api.Users/*.csproj \
    -c Release \
    --runtime linux-musl-x64 \
    --no-build \
    --no-restore \
    -o /publish



FROM base-env AS run-env

COPY --from=build-env /publish .

ENTRYPOINT ["dotnet", "Api.Users.dll"]
