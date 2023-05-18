FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ExecutionBenchmark.csproj", "./"]
RUN dotnet restore "./ExecutionBenchmark.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "ExecutionBenchmark.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ExecutionBenchmark.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExecutionBenchmark.dll"]
