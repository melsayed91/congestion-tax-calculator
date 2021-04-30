using System.Collections.Generic;
using TaxCalculatorService.Common;

namespace TaxCalculatorService.Data.Interfaces
{
    public interface IVehicleRepository
    {
        List<Vehicle> GetTollFreeVehiclesAsync();
    }
}