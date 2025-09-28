using Honamic.PayMaster.Domain.PaymentGatewayProfiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Honamic.PayMaster.Persistence.PaymentGatewayProfiles.EntityConfigurations;
public class PaymentGatewayProfileEntityConfiguration : IEntityTypeConfiguration<PaymentGatewayProfile>
{
    private readonly string schema;
    private readonly string tableName;

    public PaymentGatewayProfileEntityConfiguration(string schema, string tableName)
    {
        this.schema = schema;
        this.tableName = tableName;
    }

    public void Configure(EntityTypeBuilder<PaymentGatewayProfile> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.ToTable(tableName, schema);

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

        builder.Property(p => p.JsonConfigurations)
            .IsRequired();

        builder.Property(p => p.MinimumAmount)
            .HasPrecision(18, 2);

        builder.Property(p => p.MaximumAmount)
            .HasPrecision(18, 2);

    }
}
