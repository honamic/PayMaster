using Honamic.PayMaster.QueryModels.ReceiptIssuers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Honamic.PayMaster.Persistence.ReceiptIssuers.EntityConfigurations;
public class ReceiptIssuerQueryModelEntityConfiguration : IEntityTypeConfiguration<ReceiptIssuerQueryModel>
{
    private readonly string schema;
    private readonly string tableName;

    public ReceiptIssuerQueryModelEntityConfiguration(string schema, string tableName)
    {
        this.schema = schema;
        this.tableName = tableName;
    }

    public void Configure(EntityTypeBuilder<ReceiptIssuerQueryModel> builder)
    {
        builder.HasKey(o => o.Id); 

        builder.ToTable(tableName, schema);
    }
}
