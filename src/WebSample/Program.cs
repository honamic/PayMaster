using Honamic.PayMaster.PaymentProvider.PayPal.Extensions;
using Honamic.PayMaster.PaymentProvider.ZarinPal.Extensions;
using Microsoft.EntityFrameworkCore;
using WebSample;
using WebSample.Entities;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddZarinPalPaymentProviderServices();
        builder.Services.AddPayPalPaymentProviderServices();
        builder.Services.AddHttpClient();
        var connectionString = "";
        builder.Services.AddDbContext<SampleDbContext>((serviceProvider, options) =>
        {
            options.UseSqlServer(connectionString);
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        PaymentEndpoints.MapPaymentEndpoints(app);

        app.Run();
    }
}