using Honamic.Framework.EntityFramework.QueryModels;
using Honamic.PayMaster.QueryModels.EntityFramework.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Honamic.PayMaster.Wrapper.QueryModels;
public class PaymasterQueryModelDbContext : QueryDbContextBase
{
    public PaymasterQueryModelDbContext(DbContextOptions<PaymasterQueryModelDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.AddPayMasterQueryModels();

        base.OnModelCreating(modelBuilder);
    }
}