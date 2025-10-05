using AutoFixture.Xunit3;
using CarParkManager.Application.Models;
using CarParkManager.Application.Repositories;
using CarParkManager.Application.Services;
using CarParkManager.Domain;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;

namespace CarParkManager.Tests;

public class ParkingServiceTests
{
    private readonly ParkingRateSettings _settings = new();
    private const string VehicleReg = "ABC123";

    [Fact]
    public async Task AllocateAsync_Should_Allocate_Space()
    {
        // Arrange
        var spaceRepo = new Mock<IParkingSpaceRepository>();
        var sessionRepo = new Mock<IParkingSessionRepository>();
        spaceRepo.Setup(x => x.GetFirstAvailableSpaceAsync())
            .ReturnsAsync(new ParkingSpace
            {
                SpaceNumber = 1,
                IsOccupied = false
            });

        var service = new ParkingService(spaceRepo.Object, sessionRepo.Object,
            new ChargeCalculator(Options.Create(_settings)));

        var request = new AllocateRequest { VehicleReg = VehicleReg, VehicleType = VehicleType.Small };

        // Act
        var result = await service.AllocateAsync(request);

        // Assert
        result.ShouldNotBe(null);
        result!.SpaceNumber.ShouldBe(1);
        spaceRepo.Verify(x => x.UpdateSpaceAsync(It.IsAny<ParkingSpace>()), Times.Once);
        sessionRepo.Verify(x => x.AddSessionAsync(It.IsAny<ParkingSession>()), Times.Once);
    }

    [Fact]
    public async Task AllocateAsync_Should_Throw_When_Vehicle_Already_Parked()
    {
        // Arrange
        var spaceRepo = new Mock<IParkingSpaceRepository>();
        var sessionRepo = new Mock<IParkingSessionRepository>();

        sessionRepo.Setup(x => x.GetActiveSessionAsync(VehicleReg))
            .ReturnsAsync(new ParkingSession { VehicleReg = VehicleReg });

        var service = new ParkingService(spaceRepo.Object, sessionRepo.Object,
        new ChargeCalculator(Options.Create(_settings)));

        var request = new AllocateRequest
        {
            VehicleReg = VehicleReg,
            VehicleType = VehicleType.Small
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.AllocateAsync(request));
    }

    [Fact]
    public async Task AllocateAsync_Should_Return_Null_When_No_Space()
    {
        // Arrange
        var spaceRepo = new Mock<IParkingSpaceRepository>();
        var sessionRepo = new Mock<IParkingSessionRepository>();

        spaceRepo.Setup(x => x.GetFirstAvailableSpaceAsync())
            .ReturnsAsync((ParkingSpace?)null);
        sessionRepo.Setup(x => x.GetActiveSessionAsync(VehicleReg))
            .ReturnsAsync((ParkingSession?)null);

        var service = new ParkingService(spaceRepo.Object, sessionRepo.Object,
             new ChargeCalculator(Options.Create(_settings)));
        var request = new AllocateRequest
        {
            VehicleReg = VehicleReg,
            VehicleType = VehicleType.Small
        };

        // Act
        var result = await service.AllocateAsync(request);

        // Assert
        result.ShouldBe(null);
    }

    [Fact]
    public async Task GetStatusAsync_Should_Return_Correct_Counts()
    {
        // Arrange
        var spaceRepo = new Mock<IParkingSpaceRepository>();
        var sessionRepo = new Mock<IParkingSessionRepository>();
        spaceRepo.Setup(x => x.GetAllSpacesAsync())
            .ReturnsAsync([
                new ParkingSpace { SpaceNumber = 1, IsOccupied = false },
                new ParkingSpace { SpaceNumber = 2, IsOccupied = true },
                new ParkingSpace { SpaceNumber = 3, IsOccupied = true }
        ]);
        var service = new ParkingService(spaceRepo.Object, sessionRepo.Object, new ChargeCalculator(Options.Create(_settings)));

        // Act
        var result = await service.GetStatusAsync();

        // Assert
        result.AvailableSpaces.ShouldBe(1);
        result.OccupiedSpaces.ShouldBe(2);
    }

    [Theory]
    [AutoData]
    public async Task ExitAsync_Should_Use_ChargeCalculator_And_Deallocate_Space(double expectedCharge)
    {
        // Arrange
        var spaceRepo = new Mock<IParkingSpaceRepository>();
        var sessionRepo = new Mock<IParkingSessionRepository>();
        var chargeCalculator = new Mock<IChargeCalculator>();

        chargeCalculator.Setup(x => x.Calculate(It.IsAny<VehicleType>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()))
            .Returns(expectedCharge);

        var timeIn = DateTime.UtcNow.AddMinutes(-10);
        var session = new ParkingSession
        {
            VehicleReg = VehicleReg,
            SpaceNumber = 1,
            TimeIn = timeIn
        };
        var vehicle = new Vehicle
        {
            VehicleReg = VehicleReg,
            VehicleType = VehicleType.Small
        };
        var space = new ParkingSpace
        {
            SpaceNumber = 1,
            IsOccupied = true,
            ParkedVehicle = vehicle,
            CurrentSession = session
        };

        spaceRepo.Setup(x => x.GetSpaceByVehicleRegAsync(VehicleReg))
            .ReturnsAsync(space);
        spaceRepo.Setup(x => x.UpdateSpaceAsync(It.IsAny<ParkingSpace>()))
            .Returns(Task.CompletedTask);
        sessionRepo.Setup(x => x.UpdateSessionAsync(It.IsAny<ParkingSession>()))
            .Returns(Task.CompletedTask);

        var service = new ParkingService(spaceRepo.Object, sessionRepo.Object, chargeCalculator.Object);
        var request = new ExitRequest { VehicleReg = VehicleReg };

        // Act
        var result = await service.ExitAsync(request);

        // Assert
        result.ShouldNotBe(null);
        result!.VehicleReg.ShouldBe(VehicleReg);
        result.VehicleCharge.ShouldBe(expectedCharge);
        result.TimeIn.ShouldBe(timeIn);
        result.TimeOut.ShouldBeGreaterThan(timeIn);
        spaceRepo.Verify(x => x.UpdateSpaceAsync(It.IsAny<ParkingSpace>()), Times.Once);
        sessionRepo.Verify(x => x.UpdateSessionAsync(It.IsAny<ParkingSession>()), Times.Once);
        chargeCalculator.Verify(x => x.Calculate(VehicleType.Small, timeIn, result.TimeOut), Times.Once);
    }

    [Fact]
    public async Task ExitAsync_Should_Return_Null_When_Vehicle_Not_Found()
    {
        // Arrange
        var spaceRepo = new Mock<IParkingSpaceRepository>();
        var sessionRepo = new Mock<IParkingSessionRepository>();
        spaceRepo.Setup(x => x.GetSpaceByVehicleRegAsync(It.IsAny<string>()))
            .ReturnsAsync((ParkingSpace?)null);

        var service = new ParkingService(spaceRepo.Object, sessionRepo.Object,
         new ChargeCalculator(Options.Create(_settings)));
        var request = new ExitRequest { VehicleReg = VehicleReg };

        // Act
        var result = await service.ExitAsync(request);

        // Assert
        result.ShouldBe(null);
    }

    [Theory]
    [InlineAutoData(VehicleType.Small)]
    [InlineAutoData(VehicleType.Medium)]
    [InlineAutoData(VehicleType.Large)]
    public async Task ExitAsync_Should_Use_Correct_VehicleType_For_ChargeCalculator(VehicleType type, double expectedCharge)
    {
        // Arrange
        var spaceRepo = new Mock<IParkingSpaceRepository>();
        var sessionRepo = new Mock<IParkingSessionRepository>();
        var chargeCalculator = new Mock<IChargeCalculator>();

        chargeCalculator.Setup(x => x.Calculate(type, It.IsAny<DateTime>(), It.IsAny<DateTime>())
            ).Returns(expectedCharge);

        var timeIn = DateTime.UtcNow.AddMinutes(-10);
        var session = new ParkingSession
        {
            VehicleReg = VehicleReg,
            SpaceNumber = 1,
            TimeIn = timeIn
        };
        var vehicle = new Vehicle
        {
            VehicleReg = VehicleReg,
            VehicleType = type
        };
        var space = new ParkingSpace
        {
            SpaceNumber = 1,
            IsOccupied = true,
            ParkedVehicle = vehicle,
            CurrentSession = session
        };

        spaceRepo.Setup(x => x.GetSpaceByVehicleRegAsync(VehicleReg))
            .ReturnsAsync(space);
        spaceRepo.Setup(x => x.UpdateSpaceAsync(It.IsAny<ParkingSpace>()))
            .Returns(Task.CompletedTask);
        sessionRepo.Setup(x => x.UpdateSessionAsync(It.IsAny<ParkingSession>()))
            .Returns(Task.CompletedTask);

        var service = new ParkingService(spaceRepo.Object, sessionRepo.Object, chargeCalculator.Object);
        var request = new ExitRequest { VehicleReg = VehicleReg };

        // Act
        var result = await service.ExitAsync(request);

        // Assert
        result.ShouldNotBe(null);
        result!.VehicleCharge.ShouldBe(expectedCharge);
        chargeCalculator.Verify(x => x.Calculate(type, timeIn, result.TimeOut), Times.Once);
    }
}
