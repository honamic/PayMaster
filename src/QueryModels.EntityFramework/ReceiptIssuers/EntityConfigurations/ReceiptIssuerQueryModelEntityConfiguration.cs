using Honamic.PayMaster.QueryModels.ReceiptIssuers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Honamic.PayMaster.Persistence.ReceiptIssuers.EntityConfigurations;
public class ReceiptIssuerQueryModelEntityConfiguration : IEntityTypeConfiguration<ReceiptIssuerQueryModel>
{
    private string schema;

    public ReceiptIssuerQueryModelEntityConfiguration(string schema)
    {
        this.schema = schema;
    }

    public void Configure(EntityTypeBuilder<ReceiptIssuerQueryModel> builder)
    {
        builder.HasKey(o => o.Id); 

        builder.ToTable(nameof(ReceiptIssuerQueryModel), schema);
    }
}
