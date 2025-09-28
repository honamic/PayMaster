using Honamic.PayMaster.QueryModels.ReceiptRequests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Honamic.PayMaster.Persistence.ReceiptRequests.EntityConfigurations;
public class ReceiptRequestGatewayPaymentQueryModelEntityConfiguration : IEntityTypeConfiguration<ReceiptRequestGatewayPaymentQueryModel>
{
    private string schema;

    public ReceiptRequestGatewayPaymentQueryModelEntityConfiguration(string schema)
    {
        this.schema = schema;
    }

    public void Configure(EntityTypeBuilder<ReceiptRequestGatewayPaymentQueryModel> builder)
    {
        builder.ToTable(nameof(ReceiptRequestGatewayPaymentQueryModel), schema);

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.GatewayProvider)
            .WithMany()
            .HasForeignKey(x => x.GatewayProviderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
