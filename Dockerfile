FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["UserMicroservice.csproj", "./"]
RUN dotnet restore "./UserMicroservice.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "UserMicroservice.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UserMicroservice.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserMicroservice.dll"]