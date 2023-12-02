FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy AS base
WORKDIR /app
EXPOSE 5002

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /project
COPY ["/src/Consumer.API/Consumer.API.csproj", "/src/Consumer.API/"]
COPY ["/src/Consumer.API.Client/Consumer.API.Client.csproj", "/src/Consumer.API.Client/"]
COPY ["/src/Consumer.API.Contract/Consumer.API.Contract.csproj", "/src/Consumer.API.Contract/"]
COPY ["/src/Consumer.Application/Consumer.Application.csproj", "/src/Consumer.Application/"]
COPY ["/src/Consumer.Domain/Consumer.Domain.csproj", "/src/Consumer.Domain/"]
COPY ["/src/Consumer.Infrastructure/Consumer.Infrastructure.csproj", "/src/Consumer.Infrastructure/"]
RUN dotnet restore "/src/Consumer.API/Consumer.API.csproj"
COPY . .
WORKDIR "/project/src/Consumer.API"
RUN dotnet build "Consumer.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Consumer.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
 
ENTRYPOINT ["dotnet", "Consumer.API.dll"]