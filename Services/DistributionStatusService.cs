using DistributionValidator.Interfaces;
using DistributionValidator.Models;
using System.Globalization;

namespace DistributionValidator.Services
{
  public class DistributionStatusService(IDistributionRepository repository)
  {
    private readonly IDistributionRepository _repository = repository;

    public async Task<DistributionStatus?> GetStatusAsync(string clientId, string period)
    {
      if (string.IsNullOrWhiteSpace(clientId))
      {
        throw new ArgumentException("ClientID cannot be null or empty.", nameof(clientId));
      }

      if (string.IsNullOrWhiteSpace(period))
        {
            throw new ArgumentException("Period cannot be null or empty.", nameof(period));
      }

      if (!DateTime.TryParseExact(period, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
      {
        throw new ArgumentException("Period must be in yyyy-MM format.", nameof(period));
      }

      return await _repository.GetDistributionAsync(clientId, period);
    }
  }
}
