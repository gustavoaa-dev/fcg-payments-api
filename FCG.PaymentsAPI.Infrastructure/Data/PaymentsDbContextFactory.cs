using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FCG.PaymentsAPI.Infrastructure.Data;

public class PaymentsDbContextFactory : IDesignTimeDbContextFactory<PaymentsDbContext>
{
    public PaymentsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PaymentsDbContext>();
        optionsBuilder.UseSqlServer("Server=127.0.0.1;Database=FCG_Payments;User Id=sa;Password=Fcg2024Test!;Encrypt=False;TrustServerCertificate=True");

        return new PaymentsDbContext(optionsBuilder.Options);
    }
}
