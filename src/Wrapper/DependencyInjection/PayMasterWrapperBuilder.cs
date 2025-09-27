using Honamic.Framework.EntityFramework.Persistence.Extensions;
using Honamic.Framework.EntityFramework.Query;
using Honamic.Framework.EntityFramework.Query.Extensions;
using Honamic.Framework.Persistence.EntityFramework.Extensions;
using Honamic.PayMaster.Persistence.Extensions;
using Honamic.PayMaster.Wrapper.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Honamic.PayMaster.Wrapper.DependencyInjection;

public class PayMasterWrapperBuilder
{
    private readonly IServiceCollection services;

    public PayMasterWrapperBuilder(IServiceCollection services)
    {
        this.services = services;
    }


    public void UseEntityFrameWorkPersistence<TDbContext>() where TDbContext : DbContext
    {
        services.AddUnitOfWorkByEntityFramework<TDbContext>();
        services.AddPayMasterPersistenceServices();
    }

    public void UseSqlServerPersistence(string sqlServerConnection)
    {
        services.AddDbContext<PaymasterDbContext>((serviceProvider, options) =>
        {
            options.UseSqlServer(sqlServerConnection);
            options.AddPersianYeKeCommandInterceptor();
            options.AddAggregateRootVersionInterceptor(serviceProvider);
            options.AddMarkAsDeletedInterceptors();
        });

        services.AddUnitOfWorkByEntityFramework<PaymasterDbContext>();
        services.AddPayMasterPersistenceServices();
    }

    public void UseEntityFrameWorkQueryModel<TDbContext>() where TDbContext : QueryDbContext‌Base
    {
        services.AddDefaultQueryDbContext<TDbContext>();
        services.AddPayMasterPersistenceServices();
    }


    public void UseSqlServerQueryModel(string sqlServerConnection)
    {
        //throw new NotImplementedException();
    }

}
