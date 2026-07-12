namespace FCG.PaymentsAPI.Domain.Entities;

public class Payment
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid UserId { get; private set; }
    public Guid GameId { get; private set; }
    public decimal Price { get; private set; }
    public string Status { get; private set; }
    public DateTime ProcessedAt { get; private set; }

    public Payment(Guid orderId, Guid userId, Guid gameId, decimal price)
    {
        Id = Guid.NewGuid();
        OrderId = orderId;
        UserId = userId;
        GameId = gameId;
        Price = price;
        Status = "Pending";
        ProcessedAt = DateTime.UtcNow;
    }

    public void Approve()
    {
        Status = "Approved";
        ProcessedAt = DateTime.UtcNow;
    }

    public void Reject()
    {
        Status = "Rejected";
        ProcessedAt = DateTime.UtcNow;
    }
}
