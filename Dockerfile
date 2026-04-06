FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY WiSave.Incomes.slnx .
COPY src/ src/
RUN dotnet restore src/WiSave.Incomes/WiSave.Incomes.csproj
RUN dotnet publish src/WiSave.Incomes/WiSave.Incomes.csproj -c Release -o /app/publish --no-restore

FROM base AS final
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WiSave.Incomes.dll"]
