using System;

namespace TaxCalculatorService.Data.Interfaces
{
    public interface ITollFeeRepository
    {
        decimal GetPassageFeeByTimeAsync(DateTime passageTime);

        bool IsTollFreeDateAsync(DateTime passageTime);

        decimal GetMaximumDailyFeeAsync();

        TimeSpan GetPassageLeewayInterval();
    }
}