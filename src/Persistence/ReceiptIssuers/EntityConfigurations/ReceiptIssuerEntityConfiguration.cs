using Honamic.PayMaster.Domain.ReceiptIssuers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Honamic.PayMaster.Persistence.ReceiptIssuers.EntityConfigurations;
public class ReceiptIssuerEntityConfiguration : IEntityTypeConfiguration<ReceiptIssuer>
{
    private readonly string schema;
    private readonly string tableName;

    public ReceiptIssuerEntityConfiguration(string schema, string tableName)
    {
        this.schema = schema;
        this.tableName = tableName;
    }

    public void Configure(EntityTypeBuilder<ReceiptIssuer> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.ToTable(tableName, schema);

        builder.Property(p => p.Code)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(p => p.CallbackUrl)
            .HasMaxLength(256);

        builder.Property(p => p.Description)
            .HasMaxLength(1024);
    }
}
