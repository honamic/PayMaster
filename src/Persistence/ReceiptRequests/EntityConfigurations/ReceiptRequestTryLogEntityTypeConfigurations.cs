using Honamic.PayMaster.Domain.ReceiptRequests;
using Honamic.PayMaster.PaymentProviders.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Honamic.PayMaster.Persistence.ReceiptRequests.EntityConfigurations;
internal class ReceiptRequestTryLogEntityTypeConfigurations : IEntityTypeConfiguration<ReceiptRequestTryLog>
{
    private readonly string? _schema;
    private readonly string tableName;

    public ReceiptRequestTryLogEntityTypeConfigurations(string? schema, string tableName)
    {
        _schema = schema;
        this.tableName = tableName;
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

        builder.ToTable(tableName, _schema);
    }
}