using System;
using System.Collections.Generic;
using TaxCalculatorService.Logic.Interfaces;
using TaxCalculatorService.Common;
using TaxCalculatorService.Data.Interfaces;
using System.Linq;

namespace TaxCalculatorService.Logic
{
    public class TaxService:ITaxService
    {
        private readonly ITollFeeRepository _tollFeeRepository;
        private readonly IVehicleRepository _vehicleRepository;

        public TaxService(
            IVehicleRepository vehicleRepository,
            ITollFeeRepository tollFeeRepository)
        {
            _vehicleRepository = vehicleRepository;
            _tollFeeRepository = tollFeeRepository;
        }

        public  decimal GetTotalFee(Vehicle vehicleType, List<DateTime> passageDates)
        {
            var tollFreeVehicles =  _vehicleRepository.GetTollFreeVehiclesAsync();

            if (tollFreeVehicles.Contains(vehicleType) ||
                !passageDates.Any())
                return 0;

            decimal totalFee = 0;

            var distinctDates = passageDates.GroupBy(x => x.ToString("yyyyMMdd")).Select(y => y.First()).ToList();

            foreach (var distinctDate in distinctDates)
            {
                if ( _tollFeeRepository.IsTollFreeDateAsync(distinctDate))
                    continue;

                totalFee +=  GetTotalFeeForDay(passageDates.Where(p => p.Date == distinctDate.Date).ToList());
            }

            return totalFee;
        }

        private decimal GetTotalFeeForDay(List<DateTime> passageDates)
        {
            passageDates.Sort((a, b) => a.CompareTo(b));
            var leewayInterval =  _tollFeeRepository.GetPassageLeewayInterval();

            var intervalStart = passageDates.First();
            var intervalHighestFee =  _tollFeeRepository.GetPassageFeeByTimeAsync(intervalStart);
            decimal totalFee = 0;

            foreach (var passageDate in passageDates)
            {
                var passageFee =  _tollFeeRepository.GetPassageFeeByTimeAsync(passageDate);

                var diff = passageDate - intervalStart;

                if (diff <= leewayInterval)
                {
                    if (totalFee > 0) totalFee -= intervalHighestFee;
                    if (passageFee >= intervalHighestFee) intervalHighestFee = passageFee;
                    totalFee += intervalHighestFee;
                }
                else
                {
                    totalFee += passageFee;
                    intervalStart = passageDate;
                    intervalHighestFee = passageFee;
                }
            }

            var maximumDailyFee =  _tollFeeRepository.GetMaximumDailyFeeAsync();
            if (totalFee > maximumDailyFee) totalFee = maximumDailyFee;
            return totalFee;
        }
    }
}
