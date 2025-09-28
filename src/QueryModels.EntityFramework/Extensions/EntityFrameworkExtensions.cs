using Honamic.PayMaster.Persistence.ReceiptRequests.EntityConfigurations;
using Honamic.PayMaster.QueryModels.EntityFramework.PaymentGatewayProfiles.EntityConfigurations;
using Honamic.PayMaster.QueryModels.EntityFramework.ReceiptIssuers.EntityConfigurations;
using Honamic.PayMaster.QueryModels.EntityFramework.ReceiptRequests.EntityConfigurations; 
using Microsoft.EntityFrameworkCore;

namespace Honamic.PayMaster.QueryModels.EntityFramework.Extensions;

public static class EntityFrameworkExtensions
{

    public static ModelBuilder AddPayMasterQueryModels(this ModelBuilder modelBuilder)
    {
        if (modelBuilder == null)
        {
            throw new ArgumentNullException(nameof(modelBuilder));
        }

        modelBuilder.ApplyConfiguration(
            new PaymentGatewayProfileQueryModelEntityConfiguration(Constants.Schema, "PaymentGatewayProfiles"));

        modelBuilder.ApplyConfiguration(
            new ReceiptRequestQueryModelEntityConfiguration(Constants.Schema, "ReceiptRequests"));

        modelBuilder.ApplyConfiguration(
            new ReceiptIssuerQueryModelEntityConfiguration(Constants.Schema, "ReceiptIssuers"));

        modelBuilder.ApplyConfiguration(
            new ReceiptRequestGatewayPaymentQueryModelEntityConfiguration(Constants.Schema, "ReceiptRequestGatewayPayments"));

        modelBuilder.ApplyConfiguration(
            new ReceiptRequestTryLogQueryModelEntityTypeConfigurations(Constants.Schema, "ReceiptRequestTryLogs"));

        return modelBuilder;
    }
}