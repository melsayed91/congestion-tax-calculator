using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaxCalculatorService.Common.Options;
using TaxCalculatorService.Data.Interfaces;
using TaxCalculatorService.Data.Repositories;
using TaxCalculatorService.Logic.Interfaces;
using TaxCalculatorService.Logic;

namespace TaxCalculatorService.API.DependencyInjection
{
    public static class ServiceCollectionExtensionsS
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            return services
                .AddSingleton<IVehicleRepository, VehicleRepository>()
                .AddSingleton<ITollFeeRepository, TollFeeRepository>();
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            return services
                .AddSingleton<ITaxService, TaxService>();
        }

        public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .Configure<TollFreeVehicleOptions>(configuration.GetSection("Options"))
                .Configure<FeeTimeZonesOptions>(configuration.GetSection("Options"));
        }
    }
}