FROM microsoft/dotnet:3.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:3.1-sdk AS build
WORKDIR /src
COPY ["SimpleDotNetCoreApp/SimpleDotNetCoreApp.csproj", "SimpleDotNetCoreApp/"]
RUN dotnet restore "SimpleDotNetCoreApp/SimpleDotNetCoreApp.csproj"
COPY . .
WORKDIR "/src/SimpleDotNetCoreApp"
RUN dotnet build "SimpleDotNetCoreApp.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "SimpleDotNetCoreApp.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "SimpleDotNetCoreApp.dll"]
