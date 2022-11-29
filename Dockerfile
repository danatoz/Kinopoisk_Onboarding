FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["./WebApi/WebApi.csproj", "WebApi/"]
COPY ["./Common/Common.csproj", "Common/"]
COPY ["./Dal/Dal.csproj", "Dal/"]

RUN dotnet restore "WebApi/WebApi.csproj"
RUN dotnet restore "Common/Common.csproj"
RUN dotnet restore "Dal/Dal.csproj"

COPY . .
WORKDIR "/src/WebApi"
RUN dotnet build "WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApi.dll"]