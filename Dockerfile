FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY FCG.PaymentsAPI.sln .
COPY FCG.PaymentsAPI.API/FCG.PaymentsAPI.API.csproj FCG.PaymentsAPI.API/
COPY FCG.PaymentsAPI.Application/FCG.PaymentsAPI.Application.csproj FCG.PaymentsAPI.Application/
COPY FCG.PaymentsAPI.Domain/FCG.PaymentsAPI.Domain.csproj FCG.PaymentsAPI.Domain/
COPY FCG.PaymentsAPI.Infrastructure/FCG.PaymentsAPI.Infrastructure.csproj FCG.PaymentsAPI.Infrastructure/

RUN dotnet restore

COPY . .

RUN dotnet publish FCG.PaymentsAPI.sln -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "FCG.PaymentsAPI.API.dll"]
