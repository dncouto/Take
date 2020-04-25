FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ServerChat.csproj .
RUN dotnet restore ServerChat.csproj
COPY . .
RUN dotnet publish ServerChat.csproj -c release -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "ServerChat.dll"]