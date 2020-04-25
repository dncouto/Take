FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ClientChat.csproj .
RUN dotnet restore ClientChat.csproj
COPY . .
RUN dotnet publish ClientChat.csproj -c release -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "ClientChat.dll"]