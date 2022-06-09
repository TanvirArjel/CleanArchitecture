FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["BlazorWasmApp/BlazorWasmApp.csproj", "BlazorWasmApp/"]
COPY ["BlazorApps.Shared/BlazorApps.Shared.csproj", "BlazorApps.Shared/"]
RUN dotnet restore "BlazorWasmApp/BlazorWasmApp.csproj"
COPY . .
WORKDIR "/src/BlazorWasmApp"
RUN dotnet build "BlazorWasmApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BlazorWasmApp.csproj" -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY nginx.conf /etc/nginx/nginx.conf