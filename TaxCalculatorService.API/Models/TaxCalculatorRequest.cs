using System;
using TaxCalculatorService.Common;
using TaxCalculatorService.Common.Exceptions;

namespace TaxCalculatorService.API.Models
{
    public class TaxCalculatorRequest
    {
        public Vehicle VehicleType { get; set; }
        public DateTime[] PassageDates { get; set; }
        public Vehicle VehicleTypeToDomain()
        {
            try
            {
                var domainEnum = (Vehicle)Enum.Parse(typeof(Vehicle), VehicleType.ToString());

                return domainEnum;
            }
            catch (Exception)
            {
                throw new EnumCastException();
            }
        }
    }
}