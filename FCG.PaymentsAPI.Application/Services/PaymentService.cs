using FCG.PaymentsAPI.Domain.Entities;
using FCG.Shared.Events;
using FCG.PaymentsAPI.Domain.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using Prometheus;

namespace FCG.PaymentsAPI.Application.Services;

public class PaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<PaymentService> _logger;

    // Metrica de negocio: o payments-api e consumidor de fila, entao o /metrics HTTP
    // dele fica praticamente vazio -- e este contador que da sinal no dashboard.
    private static readonly Counter PagamentosProcessados = Metrics.CreateCounter(
        "fcg_payments_processados_total",
        "Pagamentos processados pela API, por status.",
        new CounterConfiguration { LabelNames = new[] { "status" } });

    public PaymentService(
        IPaymentRepository paymentRepository,
        IPublishEndpoint publishEndpoint,
        ILogger<PaymentService> logger)
    {
        _paymentRepository = paymentRepository;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task ProcessarPagamento(OrderPlacedEvent order)
    {
        var aprovado = Random.Shared.Next(1, 101) <= 80;
        var status = aprovado ? "Approved" : "Rejected";

        var payment = new Payment(order.OrderId, order.UserId, order.GameId, order.Price);

        if (aprovado)
            payment.Approve();
        else
            payment.Reject();

        await _paymentRepository.Adicionar(payment);
        await _paymentRepository.Salvar();

        var paymentProcessedEvent = new PaymentProcessedEvent
        {
            OrderId = order.OrderId,
            UserId = order.UserId,
            GameId = order.GameId,
            Status = status,
            ProcessedAt = payment.ProcessedAt
        };

        await _publishEndpoint.Publish(paymentProcessedEvent);

        _logger.LogInformation(
            "Pagamento processado - Order: {OrderId} | Game: {GameId} | User: {UserId} | Status: {Status}",
            order.OrderId, order.GameId, order.UserId, status);

        PagamentosProcessados.WithLabels(status).Inc();
    }
}
