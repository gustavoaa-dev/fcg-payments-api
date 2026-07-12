using FCG.PaymentsAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FCG.PaymentsAPI.Infrastructure.Data;

public class PaymentsDbContext : DbContext
{
    public DbSet<Payment> Payments { get; set; }

    public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Payment>()
            .Property(p => p.Price)
            .HasPrecision(10, 2);
    }
}
