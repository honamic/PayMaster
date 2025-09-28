using Honamic.PayMaster.QueryModels.PaymentGatewayProfiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Honamic.PayMaster.QueryModels.EntityFramework.PaymentGatewayProfiles.EntityConfigurations;
public class PaymentGatewayProfileQueryModelEntityConfiguration : IEntityTypeConfiguration<PaymentGatewayProfileQueryModel>
{
    private readonly string schema;
    private readonly string tableName;

    public PaymentGatewayProfileQueryModelEntityConfiguration(string schema, string tableName)
    {
        this.schema = schema;
        this.tableName = tableName;
    }

    public void Configure(EntityTypeBuilder<PaymentGatewayProfileQueryModel> builder)
    {
        builder.HasKey(p => p.Id); 

        builder.ToTable(tableName, schema);
    }
}
