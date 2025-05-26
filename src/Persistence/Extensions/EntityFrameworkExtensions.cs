using Honamic.PayMaster.Persistence.PaymentGatewayProviders.EntityConfigurations;
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
            new PaymentGatewayProviderEntityConfiguration(Constants.Schema));

        modelBuilder.ApplyConfiguration(
            new ReceiptRequestEntityConfiguration(Constants.Schema));

        modelBuilder.ApplyConfiguration(
            new ReceiptIssuerEntityConfiguration(Constants.Schema));

        modelBuilder.ApplyConfiguration(
            new ReceiptRequestGatewayPaymentEntityConfiguration(Constants.Schema));

        modelBuilder.ApplyConfiguration(
            new ReceiptRequestTryLogEntityTypeConfigurations(Constants.Schema));

        return modelBuilder;
    }
}