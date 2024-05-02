# Use the official mcr.microsoft.com/dotnet/aspnet image as a base image
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

# Use the official mcr.microsoft.com/dotnet/sdk image as a build image
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["UserMicroservice/UserMicroservice.csproj", "UserMicroservice/"]
RUN dotnet restore "UserMicroservice/UserMicroservice.csproj"
COPY . .
WORKDIR "/src/UserMicroservice"
RUN dotnet build "UserMicroservice.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "UserMicroservice.csproj" -c Release -o /app/publish

# Build runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Install MySQL client
RUN apt-get update \
    && apt-get install -y mysql-client \
    && rm -rf /var/lib/apt/lists/*

# Set up the entry point
ENTRYPOINT ["dotnet", "UserMicroservice.dll"]
