FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

RUN apk add --no-cache icu-libs=76.1-r1
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

COPY nuget.config ./
COPY packages ./packages

COPY Pedidos.Api/Pedidos.Api.csproj ./Pedidos.Api/
RUN dotnet restore ./Pedidos.Api/Pedidos.Api.csproj

COPY Pedidos.Api/. ./Pedidos.Api/

RUN dotnet publish ./Pedidos.Api/Pedidos.Api.csproj -c Release -o /app/publish --no-restore --no-self-contained

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app

RUN apk add --no-cache icu-libs=76.1-r1
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV ASPNETCORE_URLS=http://+:80

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Pedidos.Api.dll"]