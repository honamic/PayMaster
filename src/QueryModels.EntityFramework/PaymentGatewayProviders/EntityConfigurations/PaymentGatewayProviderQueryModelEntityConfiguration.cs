using Honamic.PayMaster.QueryModels.PaymentGatewayProviders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Honamic.PayMaster.Persistence.PaymentGatewayProviders.EntityConfigurations;
public class PaymentGatewayProviderQueryModelEntityConfiguration : IEntityTypeConfiguration<PaymentGatewayProviderQueryModel>
{
    private readonly string schema;
    private readonly string tableName;

    public PaymentGatewayProviderQueryModelEntityConfiguration(string schema, string tableName)
    {
        this.schema = schema;
        this.tableName = tableName;
    }

    public void Configure(EntityTypeBuilder<PaymentGatewayProviderQueryModel> builder)
    {
        builder.HasKey(p => p.Id); 

        builder.ToTable(tableName, schema);
    }
}
