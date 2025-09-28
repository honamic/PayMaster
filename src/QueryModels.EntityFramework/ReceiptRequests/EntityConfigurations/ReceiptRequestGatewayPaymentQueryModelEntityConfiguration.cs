using Honamic.PayMaster.QueryModels.ReceiptRequests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Honamic.PayMaster.Persistence.ReceiptRequests.EntityConfigurations;
public class ReceiptRequestGatewayPaymentQueryModelEntityConfiguration : IEntityTypeConfiguration<ReceiptRequestGatewayPaymentQueryModel>
{
    private readonly string schema;
    private readonly string tableName;

    public ReceiptRequestGatewayPaymentQueryModelEntityConfiguration(string schema, string tableName)
    {
        this.schema = schema;
        this.tableName = tableName;
    }

    public void Configure(EntityTypeBuilder<ReceiptRequestGatewayPaymentQueryModel> builder)
    {
        builder.ToTable(tableName, schema);

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.PaymentGatewayProfile)
            .WithMany(c=> c.ReceiptRequestGatewayPayments)
            .HasForeignKey(x => x.PaymentGatewayProfileId);
    }
}
