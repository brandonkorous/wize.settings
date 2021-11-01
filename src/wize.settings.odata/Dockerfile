FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ./ ./

RUN dotnet restore "src/wize.settings.odata/wize.settings.odata.csproj"
COPY . .
WORKDIR "src/wize.settings.odata"
RUN dotnet build "wize.settings.odata.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "wize.settings.odata.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "wize.settings.odata.dll"]
