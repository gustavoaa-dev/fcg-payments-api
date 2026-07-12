using FCG.PaymentsAPI.Domain.Entities;

namespace FCG.PaymentsAPI.Domain.Interfaces;

public interface IPaymentRepository
{
    Task Adicionar(Payment payment);
    Task Salvar();
}
