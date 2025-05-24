using Honamic.PayMaster.Domains.PaymentGatewayProviders;
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

        builder.Property(p => p.Code)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.ProviderType)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(p => p.LogoPath)
            .HasMaxLength(128);

        builder.Property(p => p.Configurations)
            .IsRequired();

        builder.Property(p => p.MinimumAmount)
            .HasPrecision(18, 2);

        builder.Property(p => p.MaximumAmount)
            .HasPrecision(18, 2);

    }
}
