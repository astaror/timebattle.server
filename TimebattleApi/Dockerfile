FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 50077
EXPOSE 44334

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY TimebattleApi/TimebattleApi.csproj TimebattleApi/
COPY GrainInterfaces/GrainInterfaces.csproj GrainInterfaces/
COPY Entities/Entities.csproj Entities/
COPY ConnectionInterfaces/ConnectionInterfaces.csproj ConnectionInterfaces/
RUN dotnet restore TimebattleApi/TimebattleApi.csproj
COPY . .
WORKDIR /src/TimebattleApi
RUN dotnet build TimebattleApi.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish TimebattleApi.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "TimebattleApi.dll"]
