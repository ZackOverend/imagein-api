# Use .NET 9 SDK and runtime images
FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build
WORKDIR /src

COPY ["imagein-api.csproj", "./"]
RUN dotnet restore "imagein-api.csproj"

COPY . .
RUN dotnet publish "imagein-api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0-preview AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "imagein-api.dll"]