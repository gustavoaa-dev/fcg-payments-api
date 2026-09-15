# FCG Payments API

Microsserviço responsável pelo processamento de pagamentos da plataforma FCG (Fiap Cloud Games).

## Funcionalidades

- Consome `OrderPlacedEvent` para processar pagamentos
- Simula aprovação de pagamento (80% de chance de aprovação)
- Publica `PaymentProcessedEvent` com o resultado (Approved/Rejected)
- Persiste o histórico de pagamentos

## Tecnologias

- .NET 8 (ASP.NET Core Web API)
- Entity Framework Core + SQL Server
- MassTransit + RabbitMQ
- prometheus-net 8.2.1 (`/metrics`)

> Este serviço é puramente orientado a eventos — não possui endpoints HTTP públicos.

## Como executar

### Pré-requisitos

- .NET SDK 8
- SQL Server (local ou container)
- RabbitMQ (local ou container)

### Executar localmente

```bash
# Nenhuma credencial é versionada: a string de conexão vem do ambiente
# (no cluster, do Secret do Kubernetes).
export ConnectionStrings__DefaultConnection='Server=127.0.0.1;Database=FCG_Payments;User Id=sa;Password=<sua-senha>;TrustServerCertificate=True'

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

O `appsettings.json` **não** carrega senha: a credencial vem só daqui — no cluster, do Secret do Kubernetes (ver [fcg-orchestration](https://github.com/gustavoaa-dev/fcg-orchestration), seção *Segredos*).

| Variável | Descrição | Padrão |
|---|---|---|
| `RABBITMQ_HOST` | Host do RabbitMQ | `localhost` |
| `ConnectionStrings__DefaultConnection` | String de conexão SQL Server | — |

## Observabilidade

O serviço é **consumidor de fila** (ver acima): não tem controllers e nenhuma chamada HTTP de negócio chega até ele. Mesmo assim ele expõe `/metrics` com a biblioteca `prometheus-net` 8.2.1 — `app.UseHttpMetrics()` e `app.MapMetrics()`, o mesmo padrão de `users-api` e `catalog-api` —, e é esse endpoint que o Prometheus raspa (alvo `payments-api:80`, job `fcg-apis`, a cada 15s).

Como o tráfego HTTP dele é praticamente nulo, quem dá sinal ao dashboard é a **métrica de negócio**: o contador `fcg_payments_processados_total{status="Approved"|"Rejected"}`, incrementado em `PaymentService.ProcessarPagamento` a cada evento `OrderPlacedEvent` processado. É ele que alimenta o painel *Pagamentos processados por status*.

> O contador é registrado já com o sufixo `_total` porque o `prometheus-net` 8.2.1 expõe a série **exatamente como registrada**. Para conferir em runtime (com o `port-forward` ativo em outro terminal):

```powershell
kubectl port-forward svc/payments-api 18084:80
curl.exe -s http://localhost:18084/metrics | Select-String 'fcg_payments'
```

O serviço **não** expõe `/health` nem declara probes de `startup`/`readiness`/`liveness`: sem endpoint HTTP de negócio, não há o que sondar por HTTP.

Os alvos e o dashboard estão no README do [fcg-orchestration](https://github.com/gustavoaa-dev/fcg-orchestration), seção *Observabilidade*.

## Fluxo de eventos

1. **Consome** `OrderPlacedEvent` do CatalogAPI
2. Processa pagamento (simulação 80/20)
3. **Publica** `PaymentProcessedEvent` com status `Approved` ou `Rejected`
