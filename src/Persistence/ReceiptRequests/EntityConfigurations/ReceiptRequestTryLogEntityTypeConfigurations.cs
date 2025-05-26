using Honamic.PayMaster.Domains.ReceiptRequests;
using Honamic.PayMaster.PaymentProviders.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Honamic.PayMaster.Persistence.ReceiptRequests.EntityConfigurations;
internal class ReceiptRequestTryLogEntityTypeConfigurations : IEntityTypeConfiguration<ReceiptRequestTryLog>
{
    private string? _schema;

    public ReceiptRequestTryLogEntityTypeConfigurations(string? schema)
    {
        _schema = schema;
    }

    public void Configure(EntityTypeBuilder<ReceiptRequestTryLog> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Data)
            .HasConversion(
                obj => JsonSerializer.Serialize(obj, new JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                }),
                value => JsonSerializer.Deserialize<PaymentProviderLogData?>(value, new JsonSerializerOptions
                {

                })
        );

        builder.ToTable(nameof(ReceiptRequestTryLog), _schema);
    }
}