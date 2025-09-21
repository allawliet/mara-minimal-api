using imas.Finance.ApiService.Application.Interfaces;
using imas.Finance.ApiService.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace imas.Finance.ApiService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IInvoiceService, InvoiceService>();
        services.AddScoped<IPaymentService, PaymentService>();

        return services;
    }
}