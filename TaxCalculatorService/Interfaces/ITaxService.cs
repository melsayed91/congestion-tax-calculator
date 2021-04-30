using System;
using System.Collections.Generic;
using TaxCalculatorService.Common;

namespace TaxCalculatorService.Logic.Interfaces
{
    public interface ITaxService
    {
        decimal GetTotalFee(Vehicle vehicleType, List<DateTime> passageDates);
    }
}