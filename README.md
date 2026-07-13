# FCG Payments API

Microsserviço responsável pelo processamento de pagamentos da plataforma FCG (Facul Cloud Games).

## Funcionalidades

- Consome `OrderPlacedEvent` para processar pagamentos
- Simula aprovação de pagamento (80% de chance de aprovação)
- Publica `PaymentProcessedEvent` com o resultado (Approved/Rejected)
- Persiste o histórico de pagamentos

## Tecnologias

- .NET 8 (ASP.NET Core Web API)
- Entity Framework Core + SQL Server
- MassTransit + RabbitMQ

> Este serviço é puramente orientado a eventos — não possui endpoints HTTP públicos.

## Como executar

### Pré-requisitos

- .NET SDK 8
- SQL Server (local ou container)
- RabbitMQ (local ou container)

### Executar localmente

```bash
dotnet run --project FCG.PaymentsAPI.API
```

### Com Docker

```bash
docker build -t fcg-payments-api .
docker run -p 5003:8080 fcg-payments-api
```

### Com Docker Compose

No repositório [fcg-orchestration](https://github.com/gustavoaa-dev/fcg-orchestration), execute:

```bash
docker-compose up -d
```

## Variáveis de ambiente

| Variável | Descrição | Padrão |
|---|---|---|
| `RABBITMQ_HOST` | Host do RabbitMQ | `localhost` |
| `ConnectionStrings__DefaultConnection` | String de conexão SQL Server | — |

## Fluxo de eventos

1. **Consome** `OrderPlacedEvent` do CatalogAPI
2. Processa pagamento (simulação 80/20)
3. **Publica** `PaymentProcessedEvent` com status `Approved` ou `Rejected`
