using Honamic.PayMaster.Domain.PaymentGatewayProfiles;
using Honamic.PayMaster.Domain.ReceiptRequests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Honamic.PayMaster.Persistence.ReceiptRequests.EntityConfigurations;
public class ReceiptRequestGatewayPaymentEntityConfiguration
    : IEntityTypeConfiguration<ReceiptRequestGatewayPayment>
{
    private readonly string schema;
    private readonly string tableName;

    public ReceiptRequestGatewayPaymentEntityConfiguration(string schema, string tableName)
    {
        this.schema = schema;
        this.tableName = tableName;
    }

    public void Configure(EntityTypeBuilder<ReceiptRequestGatewayPayment> builder)
    {
        builder.ToTable(tableName, schema);

        builder.HasKey(x => x.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(x => x.Currency)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(x => x.Status)
            .IsConcurrencyToken()
            .IsRequired();

        builder.Property(x => x.StatusDescription)
            .HasMaxLength(256);

        builder.Property(x => x.FailedReason)
            .IsRequired();

        builder.Property(x => x.PaymentGatewayProfileId)
            .IsRequired();

        builder.Property(x => x.CreateReference)
            .HasMaxLength(128);

        builder.Property(x => x.SuccessReference)
            .HasMaxLength(128);

        builder.Property(x => x.ReferenceRetrievalNumber)
            .HasMaxLength(32);

        builder.Property(x => x.TrackingNumber)
            .HasMaxLength(32);

        builder.Property(x => x.Pan)
            .HasMaxLength(20);

        builder.Property(x => x.TerminalId)
            .HasMaxLength(32);

        builder.Property(x => x.MerchantId)
            .HasMaxLength(32);

        builder.Property(x => x.RedirectAt);

        builder.Property(x => x.CallbackAt);

        builder.HasOne(typeof(PaymentGatewayProfile))
            .WithMany()
            .HasForeignKey(nameof(ReceiptRequestGatewayPayment.PaymentGatewayProfileId))
            .OnDelete(DeleteBehavior.Restrict);
    }
}
