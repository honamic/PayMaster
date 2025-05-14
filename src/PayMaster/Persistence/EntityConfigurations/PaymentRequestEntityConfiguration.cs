using Honamic.PayMaster.Core.PaymentRequests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Honamic.PayMaster.Persistence.EntityConfigurations;

public class PaymentRequestEntityConfiguration : IEntityTypeConfiguration<PaymentRequest>
{
    private string schema;

    public PaymentRequestEntityConfiguration(string schema)
    {
        this.schema = schema;
    }

    public void Configure(EntityTypeBuilder<PaymentRequest> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.ToTable(nameof(PaymentRequest), schema);

        builder.Property(p => p.Status)
            .IsRequired();

        builder.Property(p => p.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.Currency)
            .IsRequired()
            .HasMaxLength(5);

        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.Property(p => p.AdditionalData)
            .HasMaxLength(2048);

        builder.Property(p => p.MobileNumber)
            .HasMaxLength(20);

        builder.Property(p => p.NationalityCode)
            .HasMaxLength(20);

        builder.Property(p => p.Email)
            .HasMaxLength(256);

        builder.Property(p => p.PartyIdentity)
            .HasMaxLength(128);

        // Relationships
        builder.HasOne(p => p.Requester)
            .WithMany()
            .HasForeignKey(p => p.RequesterRef)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.GatewayPayments)
            .WithOne()
            .HasForeignKey("PaymentRequestId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
