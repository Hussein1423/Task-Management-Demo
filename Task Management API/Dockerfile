
# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ["Task Management API.csproj", "./"]

RUN dotnet restore "Task Management API.csproj"

COPY . .

RUN dotnet publish "Task Management API.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

# Stage 2: Run the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Task Management API.dll"]
