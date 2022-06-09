#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/ServerApp/Presentation/EmployeeManagement.Api/EmployeeManagement.Api.csproj", "src/ServerApp/Presentation/EmployeeManagement.Api/"]
COPY ["src/ServerApp/Infrastructure/EmployeeManagement.Infrastructure.Services/EmployeeManagement.Infrastructure.Services.csproj", "src/ServerApp/Infrastructure/EmployeeManagement.Infrastructure.Services/"]
COPY ["src/ServerApp/Core/EmployeeManagement.Application/EmployeeManagement.Application.csproj", "src/ServerApp/Core/EmployeeManagement.Application/"]
COPY ["src/ServerApp/Core/EmployeeManagement.Domain/EmployeeManagement.Domain.csproj", "src/ServerApp/Core/EmployeeManagement.Domain/"]
COPY ["src/ServerApp/Infrastructure/EmployeeManagement.Persistence.Cache/EmployeeManagement.Persistence.Cache.csproj", "src/ServerApp/Infrastructure/EmployeeManagement.Persistence.Cache/"]
COPY ["src/ServerApp/Infrastructure/EmployeeManagement.Persistence.RelationalDB/EmployeeManagement.Persistence.RelationalDB.csproj", "src/ServerApp/Infrastructure/EmployeeManagement.Persistence.RelationalDB/"]
RUN dotnet restore "src/ServerApp/Presentation/EmployeeManagement.Api/EmployeeManagement.Api.csproj"
COPY . .
WORKDIR "/src/src/ServerApp/Presentation/EmployeeManagement.Api"
RUN dotnet build "EmployeeManagement.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EmployeeManagement.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EmployeeManagement.Api.dll"]