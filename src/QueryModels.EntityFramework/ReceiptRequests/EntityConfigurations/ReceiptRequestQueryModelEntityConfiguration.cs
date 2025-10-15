using Honamic.PayMaster.QueryModels.ReceiptRequests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Honamic.PayMaster.QueryModels.EntityFramework.ReceiptRequests.EntityConfigurations;

public class ReceiptRequestQueryModelEntityConfiguration : IEntityTypeConfiguration<ReceiptRequestQueryModel>
{
    private readonly string schema;
    private readonly string tableName;

    public ReceiptRequestQueryModelEntityConfiguration(string schema, string tableName)
    {
        this.schema = schema;
        this.tableName = tableName;
    }

    public void Configure(EntityTypeBuilder<ReceiptRequestQueryModel> builder)
    {
        builder.HasKey(p => p.Id);

        builder.ToTable(tableName, schema);

        builder.HasOne(c => c.Issuer)
            .WithMany(c => c.ReceiptRequests)
            .HasForeignKey(p => p.IssuerId);

        builder.HasMany(p => p.GatewayPayments)
            .WithOne()
            .HasForeignKey(c => c.ReceiptRequestId);

        builder.HasMany(c => c.TryLogs)
            .WithOne()
            .HasForeignKey(c => c.ReceiptRequestId);
    }
}
