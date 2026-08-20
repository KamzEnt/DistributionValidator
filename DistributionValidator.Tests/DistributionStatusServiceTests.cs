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

    [TestCase("C101", "2026-01", "COMPLETED", 5000)]
    [TestCase("C102", "2026-02", "PENDING", 5500)]
    [TestCase("C103", "2026-03", "FAILED", 5600)]
    public async Task GetStatusAsync_ShouldReturnDistributionStatus(string clientId, string period, string status, decimal distributedAmount)
    {
      // Arrange
      var expectedDistribution = new DistributionStatus
      {
        ClientId = clientId,
        Period = period,
        Status = status,
        DistributedAmount = distributedAmount
      };

      _repository.GetDistributionAsync(clientId, period).Returns(expectedDistribution);

      // Act
      var result = await _service.GetStatusAsync(clientId, period);

      // Assert
      Assert.Multiple(() =>
      {
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Status, Is.EqualTo(status));
        Assert.That(result.DistributedAmount, Is.EqualTo(distributedAmount));
      });
      await _repository.Received(1).GetDistributionAsync(clientId, period);
    }

    [TestCase("C101", "")]
    [TestCase("", "2026-04")]
    public void GetStatusAstnc_ShouldRejectMissingData(string clientId, string period)
    {
      // Act & Assert
      Assert.ThrowsAsync<ArgumentException>(async () => await _service.GetStatusAsync(clientId, period));
      _repository.DidNotReceive().GetDistributionAsync(Arg.Any<string>(),Arg.Any<string>());
    }

    [TestCase("April-2026")]
    [TestCase("2026-13")]
    [TestCase("2026/10")]
    [TestCase("26-13")]
    public void GetStatusAsync_ShouldRejectInvalidPeriodFormat(string period)
    {
      // Act & Assert
      Assert.ThrowsAsync<ArgumentException>(async () => await _service.GetStatusAsync("C101", period));
      _repository.DidNotReceive().GetDistributionAsync(Arg.Any<string>(), Arg.Any<string>());
    }
  }
}
