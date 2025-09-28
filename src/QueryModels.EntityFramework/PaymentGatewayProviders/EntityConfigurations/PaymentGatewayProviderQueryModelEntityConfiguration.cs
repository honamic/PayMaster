using Honamic.PayMaster.QueryModels.PaymentGatewayProviders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Honamic.PayMaster.Persistence.PaymentGatewayProviders.EntityConfigurations;
public class PaymentGatewayProviderQueryModelEntityConfiguration : IEntityTypeConfiguration<PaymentGatewayProviderQueryModel>
{
    private string schema;

    public PaymentGatewayProviderQueryModelEntityConfiguration(string schema)
    {
        this.schema = schema;
    }

    public void Configure(EntityTypeBuilder<PaymentGatewayProviderQueryModel> builder)
    {
        builder.HasKey(p => p.Id); 

        builder.ToTable(nameof(PaymentGatewayProviderQueryModel), schema);
    }
}
