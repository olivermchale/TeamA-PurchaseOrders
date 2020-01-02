FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["TeamA.PurchaseOrdersAPI/TeamA.PurchaseOrdersAPI.csproj", "TeamA.PurchaseOrdersAPI/"]
COPY ["TeamA.PurchaseOrders.Models/TeamA.PurchaseOrders.Models.csproj", "TeamA.PurchaseOrders.Models/"]
COPY ["TeamA.PurchaseOrders.Data/TeamA.PurchaseOrders.Data.csproj", "TeamA.PurchaseOrders.Data/"]
COPY ["TeamA.PurchaseOrders.Repository/TeamA.PurchaseOrders.Repository.csproj", "TeamA.PurchaseOrders.Repository/"]
COPY ["TeamA.PurchaseOrders.Services/TeamA.PurchaseOrders.Services.csproj", "TeamA.PurchaseOrders.Services/"]
RUN dotnet restore "TeamA.PurchaseOrdersAPI/TeamA.PurchaseOrdersAPI.csproj"
COPY . .
WORKDIR "/src/TeamA.PurchaseOrdersAPI"
RUN dotnet build "TeamA.PurchaseOrdersAPI.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "TeamA.PurchaseOrdersAPI.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "TeamA.PurchaseOrdersAPI.dll"]