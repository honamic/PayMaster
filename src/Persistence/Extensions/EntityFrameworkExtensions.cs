using Honamic.PayMaster.Persistence.PaymentGatewayProfiles.EntityConfigurations; 
using Honamic.PayMaster.Persistence.ReceiptIssuers.EntityConfigurations;
using Honamic.PayMaster.Persistence.ReceiptRequests.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Honamic.PayMaster.Persistence.Extensions;

public static class EntityFrameworkExtensions
{
    [Obsolete("use the last version [Version1] and Create migration.", true)]
    public static ModelBuilder AddPayMasterModelsVersion0(this ModelBuilder modelBuilder)
    {
        return modelBuilder;
    }

    public static ModelBuilder AddPayMasterModelsVersion1(this ModelBuilder modelBuilder)
    {
        if (modelBuilder == null)
        {
            throw new ArgumentNullException(nameof(modelBuilder));
        }

        modelBuilder.ApplyConfiguration(
            new PaymentGatewayProfileEntityConfiguration(Constants.Schema, "PaymentGatewayProfiles"));

        modelBuilder.ApplyConfiguration(
            new ReceiptRequestEntityConfiguration(Constants.Schema, "ReceiptRequests"));

        modelBuilder.ApplyConfiguration(
            new ReceiptIssuerEntityConfiguration(Constants.Schema, "ReceiptIssuers"));

        modelBuilder.ApplyConfiguration(
            new ReceiptRequestGatewayPaymentEntityConfiguration(Constants.Schema, "ReceiptRequestGatewayPayments"));

        modelBuilder.ApplyConfiguration(
            new ReceiptRequestTryLogEntityTypeConfigurations(Constants.Schema, "ReceiptRequestTryLogs"));
         
        return modelBuilder;
    }
}