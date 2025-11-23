FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ClearHr.Blazor/ClearnHr.Blazor.csproj", "ClearnHr.Blazor/"]
RUN dotnet restore "CleanHr.Blazor/CleanHr.Blazor.csproj"
COPY . .
WORKDIR "/src/ClientApps/CleanHr.Blazor"
RUN dotnet build "CleanHr.Blazor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CleanHr.Blazor.csproj" -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY nginx.conf /etc/nginx/nginx.conf