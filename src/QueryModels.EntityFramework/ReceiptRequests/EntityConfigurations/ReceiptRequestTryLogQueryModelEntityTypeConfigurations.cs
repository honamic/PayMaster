using Honamic.PayMaster.PaymentProviders.Models;
using Honamic.PayMaster.QueryModels.ReceiptRequests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Honamic.PayMaster.Persistence.ReceiptRequests.EntityConfigurations;
internal class ReceiptRequestTryLogQueryModelEntityTypeConfigurations : IEntityTypeConfiguration<ReceiptRequestTryLogQueryModel>
{
    private readonly string? _schema;
    private readonly string _tableName;

    public ReceiptRequestTryLogQueryModelEntityTypeConfigurations(string? schema, string tableName)
    {
        _schema = schema;
        _tableName = tableName;
    }

    public void Configure(EntityTypeBuilder<ReceiptRequestTryLogQueryModel> builder)
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

        builder.ToTable(_tableName, _schema);
    }
}