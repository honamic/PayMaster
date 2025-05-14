using Honamic.PayMaster.Core.PaymentRequests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Honamic.PayMaster.Persistence.EntityConfigurations;
public class PaymentRequestPaymentGatewayEntityConfiguration
    : IEntityTypeConfiguration<PaymentRequestPaymentGateway>
{
    private string schema;

    public PaymentRequestPaymentGatewayEntityConfiguration(string schema)
    {
        this.schema = schema;
    }

    public void Configure(EntityTypeBuilder<PaymentRequestPaymentGateway> builder)
    {
        builder.ToTable(nameof(PaymentRequestPaymentGateway), schema);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.Currency)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.FailedReason)
            .IsRequired();

        builder.Property(x => x.GatewayProviderRef)
            .IsRequired();

        builder.Property(x => x.GatewayCreateReference)
            .HasMaxLength(128);

        builder.Property(x => x.GatewaySuccessReference)
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

        builder.Property(x => x.GatewayRedirectAt);

        builder.Property(x => x.GatewayCallBackAt);

        builder.HasOne(x => x.GatewayProvider)
            .WithMany()
            .HasForeignKey(x => x.GatewayProviderRef)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
