using Honamic.PayMaster.Extensions;
using Microsoft.EntityFrameworkCore;

namespace WebSample.Entities;
public class SampleDbContext : DbContext
{
    public SampleDbContext(DbContextOptions<SampleDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.AddPayMasterModelsVersion1();

        base.OnModelCreating(modelBuilder);
    }
}