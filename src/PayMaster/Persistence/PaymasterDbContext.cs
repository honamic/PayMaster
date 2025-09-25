using Honamic.PayMaster.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Honamic.PayMaster.Persistence;
public class PaymasterDbContext : DbContext
{
    public PaymasterDbContext(DbContextOptions<PaymasterDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.AddPayMasterModelsVersion1();

        base.OnModelCreating(modelBuilder);
    }
}