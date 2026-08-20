using DistributionValidator.Models;

namespace DistributionValidator.Interfaces
{
  public interface IDistributionRepository
  {
    Task<DistributionStatus?> GetDistributionAsync(string clientId, string period);
  }
}
