using Honamic.Framework.EntityFramework.Persistence.Extensions;
using Honamic.Framework.EntityFramework.Query;
using Honamic.Framework.EntityFramework.Query.Extensions;
using Honamic.Framework.Persistence.EntityFramework.Extensions;
using Honamic.PayMaster.Options;
using Honamic.PayMaster.Persistence.Extensions;
using Honamic.PayMaster.Wrapper.Persistence;
using Honamic.PayMaster.Wrapper.QueryModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Honamic.PayMaster.Wrapper.DependencyInjection;

public class PayMasterWrapperBuilder
{
    public IServiceCollection Services { get; }

    internal bool PersistenceConfigured { get; private set; }
    internal bool QueryModelConfigured { get; private set; }
    public PayMasterWrapperBuilder(IServiceCollection services)
    {
        Services = services;
    }

    public void Configure(Action<PayMasterOptions> configure)
    {
        Services.Configure<PayMasterOptions>(configure);
    }

    public void UseEntityFrameWorkPersistence<TDbContext>() where TDbContext : DbContext
    {
        EnsurePersistenceNotAlreadyConfigured();

        PersistenceConfigured = true;

        Services.AddUnitOfWorkByEntityFramework<TDbContext>();
        Services.AddPayMasterPersistenceServices();
    }

    public void UseSqlServerPersistence(string sqlServerConnection)
    {
        EnsurePersistenceNotAlreadyConfigured();

        PersistenceConfigured = true;

        Services.AddDbContext<PaymasterDbContext>((serviceProvider, options) =>
        {
            options.UseSqlServer(sqlServerConnection);
            options.AddPersianYeKeCommandInterceptor();
            options.AddAggregateRootVersionInterceptor(serviceProvider);
            options.AddMarkAsDeletedInterceptors();
        });

        Services.AddUnitOfWorkByEntityFramework<PaymasterDbContext>();
        Services.AddPayMasterPersistenceServices();
    }

    public void UseEntityFrameWorkQueryModel<TDbContext>() where TDbContext : QueryDbContext‌Base
    {
        EnsureQueryModelNotAlreadyConfigured();
        QueryModelConfigured = true;

        Services.AddDefaultQueryDbContext<TDbContext>();
        Services.AddPayMasterPersistenceServices();
    }


    public void UseSqlServerQueryModel(string sqlServerConnection)
    {
        EnsureQueryModelNotAlreadyConfigured();
        QueryModelConfigured = true;

        Services.AddDbContext<PaymasterQueryModelDbContext>((serviceProvider, options) =>
        {
            options.UseSqlServer(sqlServerConnection);
            options.AddPersianYeKeCommandInterceptor();
        });

        Services.AddDefaultQueryDbContext<PaymasterQueryModelDbContext>();
    }


    private void EnsurePersistenceNotAlreadyConfigured()
    {
        if (PersistenceConfigured)
            throw new InvalidOperationException("Persistence already configured. You cannot configure it twice.");
    }

    private void EnsureQueryModelNotAlreadyConfigured()
    {
        if (QueryModelConfigured)
            throw new InvalidOperationException("QueryModel already configured. You cannot configure it twice.");
    }
}