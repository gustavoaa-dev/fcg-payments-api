using FCG.Shared.Events;
using FCG.PaymentsAPI.Application.Services;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FCG.PaymentsAPI.Application.Consumers;

public class OrderPlacedConsumer : IConsumer<OrderPlacedEvent>
{
    private readonly PaymentService _paymentService;
    private readonly ILogger<OrderPlacedConsumer> _logger;

    public OrderPlacedConsumer(PaymentService paymentService, ILogger<OrderPlacedConsumer> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderPlacedEvent> context)
    {
        var order = context.Message;

        _logger.LogInformation(
            "Pedido recebido - OrderId: {OrderId} | GameId: {GameId} | Price: {Price:C}",
            order.OrderId, order.GameId, order.Price);

        await _paymentService.ProcessarPagamento(order);
    }
}
