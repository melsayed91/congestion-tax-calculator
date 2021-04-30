﻿using System;
using TaxCalculatorService.Data.Interfaces;
using AutoFixture;
using NSubstitute;
using TaxCalculatorService.Common;
using System.Collections.Generic;
using Xunit;

namespace TaxCalculatorService.Logic.UnitTests
{
    public class TaxServiceTests : UnitTestBase<TaxService>
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly ITollFeeRepository _tollFeeRepository;
        private const decimal MAXIMUM_DAILY_FEE = 60;
        private readonly TimeSpan _passageLeewayInterval = TimeSpan.FromMinutes(60);


        public TaxServiceTests()
        {
            _vehicleRepository = Fixture.Freeze<IVehicleRepository>();
            _tollFeeRepository = Fixture.Freeze<ITollFeeRepository>();

            _vehicleRepository.GetTollFreeVehiclesAsync().Returns(new List<Vehicle>
            {
                Vehicle.Diplomat,
                Vehicle.Emergency,
                Vehicle.Foreign,
                Vehicle.Military,
                Vehicle.Motorbike,
                Vehicle.Bus
            });

            _tollFeeRepository.GetMaximumDailyFeeAsync().Returns(MAXIMUM_DAILY_FEE);
            _tollFeeRepository.GetPassageLeewayInterval().Returns(_passageLeewayInterval);
        }
        [Fact]
        public void ForGetTotalFee_When2PassagesMoreThanAnHourApart_ReturnFeeForBothPassages()
        {
            var vehicleType = Vehicle.Car;
            var passageFee = MAXIMUM_DAILY_FEE / 8;
            var passage1 = new DateTime(1, 1, 1, 12, 50, 0);
            var passage2 = new DateTime(1, 1, 1, 14, 50, 0);
            var passageDates = new List<DateTime>
            {
                passage1,
                passage2
            };

            _tollFeeRepository.GetPassageFeeByTimeAsync(Arg.Is(passage1)).Returns(passageFee);
            _tollFeeRepository.GetPassageFeeByTimeAsync(Arg.Is(passage2)).Returns(passageFee);
            _tollFeeRepository.IsTollFreeDateAsync(Arg.Any<DateTime>()).Returns(false);

            var response = SUT.GetTotalFee(vehicleType, passageDates);

            Assert.Equal(passageFee * 2, response);
        }

        [Fact]
        public void ForGetTotalFee_When3PassagesAnd2LessThanAnHourApart_ReturnCorrectCombinedFee()
        {
            var vehicleType = Vehicle.Car;
            var cheaperFee = MAXIMUM_DAILY_FEE / 8;
            var moreExpensiveFee = MAXIMUM_DAILY_FEE / 6;
            var passage1 = new DateTime(1, 1, 1, 12, 50, 0);
            var passage2 = new DateTime(1, 1, 1, 12, 55, 0);
            var passage3 = new DateTime(1, 1, 1, 15, 55, 0);
            var passageDates = new List<DateTime>
            {
                passage1,
                passage2,
                passage3
            };

            _tollFeeRepository.GetPassageFeeByTimeAsync(Arg.Is(passage1)).Returns(cheaperFee);
            _tollFeeRepository.GetPassageFeeByTimeAsync(Arg.Is(passage2)).Returns(moreExpensiveFee);
            _tollFeeRepository.GetPassageFeeByTimeAsync(Arg.Is(passage3)).Returns(cheaperFee);
            _tollFeeRepository.IsTollFreeDateAsync(Arg.Any<DateTime>()).Returns(false);

            var response =  SUT.GetTotalFee(vehicleType, passageDates);

            Assert.Equal(moreExpensiveFee + cheaperFee, response);
        }

        [Fact]
        public void ForGetTotalFee_WhenOneDateIsTollFree_ReturnNoFeeForThatDay()
        {
            var vehicleType = Vehicle.Car;
            var passageFee = MAXIMUM_DAILY_FEE / 8;
            var passage1 = new DateTime(1, 1, 1, 12, 50, 0);
            var tollFreeDate = new DateTime(2, 2, 2, 12, 50, 0);
            var passageDates = new List<DateTime>
            {
                passage1,
                tollFreeDate
            };

            _tollFeeRepository.GetPassageFeeByTimeAsync(Arg.Is(passage1)).Returns(passageFee);
            _tollFeeRepository.IsTollFreeDateAsync(Arg.Is(passage1)).Returns(false);
            _tollFeeRepository.IsTollFreeDateAsync(Arg.Is(tollFreeDate)).Returns(true);

            var response = SUT.GetTotalFee(vehicleType, passageDates);
            Assert.Equal(passageFee, response);
        }

        [Fact]
        public void ForGetTotalFee_WhenOneTimeDuringDay_ReturnFee()
        {
            var vehicleType = Vehicle.Car;
            var passageFee = MAXIMUM_DAILY_FEE / 8;
            var passage1 = new DateTime(1, 1, 1, 12, 50, 0);
            var passageDates = new List<DateTime>
            {
                passage1
            };

            _tollFeeRepository.GetPassageFeeByTimeAsync(Arg.Is(passage1)).Returns(passageFee);
            _tollFeeRepository.IsTollFreeDateAsync(Arg.Is(passage1)).Returns(false);

            var response = SUT.GetTotalFee(vehicleType, passageDates);
            Assert.Equal(passageFee, response);
        }

        [Fact]
        public void ForGetTotalFee_WhenTotalDailyFeeExceedsMaximum_ReturnMaximumForThatDay()
        {
            var vehicleType = Vehicle.Car;
            decimal passageFee = (MAXIMUM_DAILY_FEE / 2) + (MAXIMUM_DAILY_FEE / 4);
            var passage1Day1 = new DateTime(1, 1, 1, 12, 50, 0);
            var passage2Day1 = new DateTime(1, 1, 1, 16, 55, 0);
            var passage1Day2 = new DateTime(1, 1, 2, 15, 55, 0);
            var passageDates = new List<DateTime>
            {
                passage1Day1,
                passage2Day1,
                passage1Day2
            };

            _tollFeeRepository.GetPassageFeeByTimeAsync(Arg.Any<DateTime>()).Returns(passageFee);
            _tollFeeRepository.IsTollFreeDateAsync(Arg.Any<DateTime>()).Returns(false);

            var response =  SUT.GetTotalFee(vehicleType, passageDates);
            Assert.Equal(MAXIMUM_DAILY_FEE + passageFee,response);
        }

        [Fact]
        public void ForGetTotalFee_WhenVehicleIsTollFree_ReturnNoFee()
        {
            var vehicleType = Vehicle.Emergency;
            var passageDates = Fixture.Create<List<DateTime>>();

            var response = SUT.GetTotalFee(vehicleType, passageDates);
            Assert.Equal(0, response);
        }
    }
}
