using Honamic.PayMaster.Core.PaymentRequesters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Honamic.PayMaster.Persistence.EntityConfigurations;
public class PaymentRequesterEntityConfiguration : IEntityTypeConfiguration<PaymentRequester>
{
    private string schema;

    public PaymentRequesterEntityConfiguration(string schema)
    {
        this.schema = schema;
    }

    public void Configure(EntityTypeBuilder<PaymentRequester> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.ToTable(nameof(PaymentRequester), schema);


        builder.Property(p => p.Code)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.ShowPaymentResultPage)
            .IsRequired();

        builder.Property(p => p.CallbackUrl)
            .HasMaxLength(500);

        builder.Property(p => p.WebHookUrl)
            .HasMaxLength(500);

        builder.Property(p => p.Description)
            .HasMaxLength(1000);
    }
}
