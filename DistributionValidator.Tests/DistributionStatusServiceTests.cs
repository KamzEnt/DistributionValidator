using DistributionValidator.Interfaces;
using DistributionValidator.Models;
using DistributionValidator.Services;
using NSubstitute;

namespace DistributionValidator.Tests
{
  [TestFixture]
  public class DistributionStatusServiceTests
  {
    private IDistributionRepository _repository = null!;
    private DistributionStatusService _service = null!;

    [SetUp]
    public void SetUp()
    {
      _repository = Substitute.For<IDistributionRepository>();
      _service = new DistributionStatusService(_repository);
    }

    [Test]
    public async Task GetStatusCompletedAsync_ShouldReturnDustributionStatus()
    {
      // Arrange
      var expectedDistribution = new DistributionStatus
      {
        ClientId = "C101",
        Period = "2026-04",
        Status = "COMPLETED",
        DistributedAmount = 7840.00m
      };

      _repository.GetDistributionAsync("C101", "2026-04").Returns(expectedDistribution);

      // Act
      var result = await _service.GetStatusAsync("C101", "2026-04");

      // Assert
      await _repository.Received(1).GetDistributionAsync("C101", "2026-04");
      Assert.Multiple(() =>
      {
        Assert.That(result?.Status, Is.EqualTo("COMPLETED"));
        Assert.That(result?.DistributedAmount, Is.EqualTo(7840.00m));
      });
    }

    [Test]
    public async Task GetStatusPendingAsync_ShouldReturnDustributionStatus()
    {
      // Arrange
      var expectedDistribution = new DistributionStatus
      {
        ClientId = "C102",
        Period = "2026-05",
        Status = "PENDING",
        DistributedAmount = 7840.00m
      };

      _repository.GetDistributionAsync("C102", "2026-05").Returns(expectedDistribution);

      // Act
      var result = await _service.GetStatusAsync("C102", "2026-05");

      // Assert
      await _repository.Received(1).GetDistributionAsync("C102", "2026-05");
      Assert.Multiple(() =>
      {
        Assert.That(result?.Status, Is.EqualTo("PENDING"));
        Assert.That(result?.DistributedAmount, Is.EqualTo(7840.00m));
      });
    }

    [Test]
    public async Task GetStatusFailedAsync_ShouldReturnDustributionStatus()
    {
      // Arrange
      var expectedDistribution = new DistributionStatus
      {
        ClientId = "C102",
        Period = "2026-05",
        Status = "FAILED",
        DistributedAmount = 7840.00m
      };

      _repository.GetDistributionAsync("C102", "2026-05").Returns(expectedDistribution);

      // Act
      var result = await _service.GetStatusAsync("C102", "2026-05");

      // Assert
      await _repository.Received(1).GetDistributionAsync("C102", "2026-05");
      Assert.Multiple(() =>
      {
        Assert.That(result?.Status, Is.EqualTo("FAILED"));
        Assert.That(result?.DistributedAmount, Is.EqualTo(7840.00m));
      });
    }

    [Test]
    public void EmptyPeriod_ShouldThrowArgumentException()
    {
      // Act & Assert
      Assert.ThrowsAsync<ArgumentException>(async () => await _service.GetStatusAsync("C101", ""));
      _repository.DidNotReceive().GetDistributionAsync(Arg.Any<string>(),Arg.Any<string>());
    }

    [Test]
    public void EmptyClientId_ShouldThrowArgumentException()
    {
      // Act & Assert
      Assert.ThrowsAsync<ArgumentException>(async () => await _service.GetStatusAsync("", "2026-04"));
      _repository.DidNotReceive().GetDistributionAsync(Arg.Any<string>(), Arg.Any<string>());
    }

    [TestCase("April-2026")]
    [TestCase("2026-13")]
    [TestCase("26-13")]
    public void GetStatusAsync_ShouldRejectInvalidPeriodFormat(string period)
    {
      // Act & Assert
      Assert.ThrowsAsync<ArgumentException>(async () => await _service.GetStatusAsync("C101", period));
      _repository.DidNotReceive().GetDistributionAsync(Arg.Any<string>(), Arg.Any<string>());
    }
  }
}
