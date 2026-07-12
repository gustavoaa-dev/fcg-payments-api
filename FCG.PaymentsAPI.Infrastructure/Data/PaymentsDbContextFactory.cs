using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FCG.PaymentsAPI.Infrastructure.Data;

public class PaymentsDbContextFactory : IDesignTimeDbContextFactory<PaymentsDbContext>
{
    public PaymentsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PaymentsDbContext>();
        optionsBuilder.UseSqlServer("Server=localhost;Database=FCG_Payments;Trusted_Connection=True;TrustServerCertificate=True");

        return new PaymentsDbContext(optionsBuilder.Options);
    }
}
