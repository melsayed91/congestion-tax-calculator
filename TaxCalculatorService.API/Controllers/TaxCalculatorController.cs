using System;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TaxCalculatorService.Logic.Interfaces;
using TaxCalculatorService.API.Models;
using TaxCalculatorService.Common.Exceptions;

namespace TaxCalculatorService.API.Controllers
{
    [ApiController]
    public class TaxCalculatorController : ControllerBase
    {
        private readonly ITaxService _taxService;

        public TaxCalculatorController(
            ITaxService taxService)
        {
            _taxService = taxService;
        }

        /// <summary>
        /// Get the total fee for all passages with provided vehicle
        /// </summary>
        /// <param name="requestModel">PassageDates eg. 2021-04-07T14:25:00</param>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [HttpGet("api/tax")]
        public async Task<IActionResult> GetTotalFee([FromQuery]  TaxCalculatorRequest requestModel)
        {
            try
            {
                var totalFee = _taxService.GetTotalFee(requestModel.VehicleTypeToDomain(), requestModel.PassageDates.ToList());

                return Ok(new TaxCalculatorResponse
                {
                    TotalFee = totalFee
                });
            }
            catch (EnumCastException)
            {
                return BadRequest("Unable to parse VehicleType input");
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e);
            }
        }

    }
}