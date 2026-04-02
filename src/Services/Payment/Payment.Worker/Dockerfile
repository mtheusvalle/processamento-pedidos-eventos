# Etapa 1: Build 
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["ProcessamentoPedidos.slnx", "./"]
COPY ["BuildingBlocks/ProcessamentoPedidos.Core/ProcessamentoPedidos.Core.csproj", "BuildingBlocks/ProcessamentoPedidos.Core/"]
COPY ["Services/Payment/Payment.Worker/Payment.Worker.csproj", "Services/Payment/Payment.Worker/"]

RUN dotnet restore "Services/Payment/Payment.Worker/Payment.Worker.csproj"

# Publica o Worker
COPY . .
WORKDIR "/src/Services/Payment/Payment.Worker"
RUN dotnet publish "Payment.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 2: Runtime 
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Payment.Worker.dll"]
