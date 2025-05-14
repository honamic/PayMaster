using Honamic.PayMaster.Core.PaymentGatewayProviders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Honamic.PayMaster.Persistence.EntityConfigurations;
public class PaymentGatewayProviderEntityConfiguration : IEntityTypeConfiguration<PaymentGatewayProvider>
{
    private string schema;

    public PaymentGatewayProviderEntityConfiguration(string schema)
    {
        this.schema = schema;
    }

    public void Configure(EntityTypeBuilder<PaymentGatewayProvider> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.ToTable(nameof(PaymentGatewayProvider), schema);


        builder.Property(p => p.ProviderType)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(p => p.LogoPath)
            .HasMaxLength(500);

        builder.Property(p => p.Configurations)
            .IsRequired();

        builder.Property(p => p.Enable)
            .IsRequired();

        builder.Property(p => p.Order)
            .IsRequired();
    }
}
