FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 57525
EXPOSE 44322

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ManagementApi/ManagementApi.csproj ManagementApi/
COPY GrainInterfaces/GrainInterfaces.csproj GrainInterfaces/
COPY Entities/Entities.csproj Entities/
RUN dotnet restore ManagementApi/ManagementApi.csproj
COPY . .
WORKDIR /src/ManagementApi
RUN dotnet build ManagementApi.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish ManagementApi.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ManagementApi.dll"]
