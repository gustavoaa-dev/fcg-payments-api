using FCG.PaymentsAPI.Domain.Entities;
using FCG.PaymentsAPI.Domain.Interfaces;
using FCG.PaymentsAPI.Infrastructure.Data;

namespace FCG.PaymentsAPI.Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly PaymentsDbContext _context;

    public PaymentRepository(PaymentsDbContext context)
    {
        _context = context;
    }

    public async Task Adicionar(Payment payment)
    {
        await _context.Payments.AddAsync(payment);
    }

    public async Task Salvar()
    {
        await _context.SaveChangesAsync();
    }
}
