using TaxCalculatorService.Data.Interfaces;
using TaxCalculatorService.Common.Options;
using System.Collections.Generic;
using TaxCalculatorService.Common;
using Microsoft.Extensions.Options;

namespace TaxCalculatorService.Data.Repositories
{
    public class VehicleRepository: IVehicleRepository
    {
        private readonly TollFreeVehicleOptions _tollFreeVehicleOptions;

        public VehicleRepository(IOptions<TollFreeVehicleOptions> tollFreeVehicleOptions)
        {
            _tollFreeVehicleOptions = tollFreeVehicleOptions.Value;
        }

        public  List<Vehicle> GetTollFreeVehiclesAsync()
        {
            return _tollFreeVehicleOptions.TollFreeVehicles;
        }
    }
}