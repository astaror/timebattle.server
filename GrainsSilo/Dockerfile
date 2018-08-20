FROM microsoft/dotnet:2.1-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY InternalMessages/InternalMessages.csproj InternalMessages/
COPY GrainsSilo/GrainsSilo.csproj GrainsSilo/
COPY GrainInterfaces/GrainInterfaces.csproj GrainInterfaces/
COPY Entities/Entities.csproj Entities/
COPY Grains/Grains.csproj Grains/
RUN dotnet restore GrainsSilo/GrainsSilo.csproj
COPY . .
WORKDIR /src/GrainsSilo
RUN dotnet build GrainsSilo.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish GrainsSilo.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "GrainsSilo.dll"]
