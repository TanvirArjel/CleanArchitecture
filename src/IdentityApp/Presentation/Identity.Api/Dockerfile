#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/IdentityApp/Presentation/Identity.Api/Identity.Api.csproj", "src/IdentityApp/Presentation/Identity.Api/"]
COPY ["src/IdentityApp/Infrastructure/Identity.Persistence.RelationalDB/Identity.Persistence.RelationalDB.csproj", "src/IdentityApp/Infrastructure/Identity.Persistence.RelationalDB/"]
COPY ["src/IdentityApp/Core/Identity.Domain/Identity.Domain.csproj", "src/IdentityApp/Core/Identity.Domain/"]
COPY ["src/IdentityApp/Infrastructure/Identity.Infrastructure.Services/Identity.Infrastructure.Services.csproj", "src/IdentityApp/Infrastructure/Identity.Infrastructure.Services/"]
COPY ["src/IdentityApp/Core/Identity.Application/Identity.Application.csproj", "src/IdentityApp/Core/Identity.Application/"]
RUN dotnet restore "src/IdentityApp/Presentation/Identity.Api/Identity.Api.csproj"
COPY . .
WORKDIR "/src/src/IdentityApp/Presentation/Identity.Api"
RUN dotnet build "Identity.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Identity.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Identity.Api.dll"]