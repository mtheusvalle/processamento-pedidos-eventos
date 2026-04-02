# Etapa 1: Build (Usa o SDK completo)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiamos o solution e os csproj preservando a raiz limpa
COPY ["ProcessamentoPedidos.slnx", "./"]
COPY ["BuildingBlocks/ProcessamentoPedidos.Core/ProcessamentoPedidos.Core.csproj", "BuildingBlocks/ProcessamentoPedidos.Core/"]
COPY ["Services/Checkout/Checkout.API/Checkout.API.csproj", "Services/Checkout/Checkout.API/"]
COPY ["Services/Checkout/Checkout.Application/Checkout.Application.csproj", "Services/Checkout/Checkout.Application/"]
COPY ["Services/Checkout/Checkout.Domain/Checkout.Domain.csproj", "Services/Checkout/Checkout.Domain/"]
COPY ["Services/Checkout/Checkout.Infrastructure/Checkout.Infrastructure.csproj", "Services/Checkout/Checkout.Infrastructure/"]
COPY ["Services/Checkout/Checkout.Data/Checkout.Data.csproj", "Services/Checkout/Checkout.Data/"]

RUN dotnet restore "Services/Checkout/Checkout.API/Checkout.API.csproj"

# Agora copiamos todo o código e publicamos
COPY . .
WORKDIR "/src/Services/Checkout/Checkout.API"
RUN dotnet publish "Checkout.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 2: Runtime (Usa a imagem enxuta apenas com o ambiente de execução)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Checkout.API.dll"]
