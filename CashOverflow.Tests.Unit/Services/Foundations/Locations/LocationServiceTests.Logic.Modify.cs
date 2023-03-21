﻿// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by CashOverflow Team
// --------------------------------------------------------

using System;
using System.Threading.Tasks;
using Moq;
using Xunit;
using CashOverflow.Models.Locations;
using Force.DeepCloner;
using FluentAssertions;

namespace CashOverflow.Tests.Unit.Services.Foundations.Locations
{
    public partial class LocationServiceTests
    {
        [Fact]
        public async Task ShouldModifyLocationAsync()
        {
            //given
            DateTimeOffset randomDate = GetRandomDateTimeOffset();
            Location randomLocation = CreateRandomModifyLocation(randomDate);
            Location inputLocation = randomLocation;
            Location storageLocation = inputLocation.DeepClone();
            storageLocation.UpdatedDate = randomLocation.CreatedDate;
            Location updatedLocation = inputLocation;
            Location expectedLocation = updatedLocation.DeepClone();
            Guid LocationId = inputLocation.Id;

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDate);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectLocationByIdAsync(LocationId))
                    .ReturnsAsync(storageLocation);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateLocationAsync(inputLocation))
                    .ReturnsAsync(updatedLocation);

            //when
            Location actualLocation =
                await this.locationService.ModifyLocationAsync(inputLocation);

            //then
            actualLocation.Should().BeEquivalentTo(expectedLocation);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectLocationByIdAsync(LocationId), Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateLocationAsync(inputLocation), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
