using Honamic.PayMaster.Core.ReceiptRequests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Honamic.PayMaster.Persistence.EntityConfigurations;

public class ReceiptRequestEntityConfiguration : IEntityTypeConfiguration<ReceiptRequest>
{
    private string schema;

    public ReceiptRequestEntityConfiguration(string schema)
    {
        this.schema = schema;
    }

    public void Configure(EntityTypeBuilder<ReceiptRequest> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.ToTable(nameof(ReceiptRequest), schema);

        builder.Property(p => p.Status)
            .IsRequired();

        builder.Property(p => p.Amount)
            .IsRequired()
            .HasPrecision(18, 2);
        

        builder.Property(p => p.Currency)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(p => p.Description)
            .HasMaxLength(256);

        builder.Property(p => p.AdditionalData)
            .HasMaxLength(2048);

        builder.Property(p => p.Mobile)
            .HasMaxLength(16);

        builder.Property(p => p.NationalityCode)
            .HasMaxLength(16);

        builder.Property(p => p.Email)
            .HasMaxLength(256);

        builder.Property(p => p.PartyIdentity)
            .HasMaxLength(128);

        // Relationships
        builder.HasOne(p => p.Issuer)
            .WithMany()
            .HasForeignKey(p => p.IssuerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.GatewayPayments)
            .WithOne()
            .HasForeignKey("PaymentRequestId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.TryLogs)
            .WithOne()
            .HasForeignKey(c => c.ReceiptRequestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
