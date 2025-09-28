using Honamic.PayMaster.Persistence.PaymentGatewayProviders.EntityConfigurations;
using Honamic.PayMaster.Persistence.ReceiptIssuers.EntityConfigurations;
using Honamic.PayMaster.Persistence.ReceiptRequests.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Honamic.PayMaster.Persistence.Extensions;

public static class EntityFrameworkExtensions
{

    public static ModelBuilder AddPayMasterQueryModels(this ModelBuilder modelBuilder)
    {
        if (modelBuilder == null)
        {
            throw new ArgumentNullException(nameof(modelBuilder));
        }

        modelBuilder.ApplyConfiguration(
            new PaymentGatewayProviderQueryModelEntityConfiguration(Constants.Schema, "PaymentGatewayProvider"));

        modelBuilder.ApplyConfiguration(
            new ReceiptRequestQueryModelEntityConfiguration(Constants.Schema, "ReceiptRequest"));

        modelBuilder.ApplyConfiguration(
            new ReceiptIssuerQueryModelEntityConfiguration(Constants.Schema, "ReceiptIssuer"));

        modelBuilder.ApplyConfiguration(
            new ReceiptRequestGatewayPaymentQueryModelEntityConfiguration(Constants.Schema, "ReceiptRequestGatewayPayment"));

        modelBuilder.ApplyConfiguration(
            new ReceiptRequestTryLogQueryModelEntityTypeConfigurations(Constants.Schema, "ReceiptRequestTryLog"));

        return modelBuilder;
    }
}