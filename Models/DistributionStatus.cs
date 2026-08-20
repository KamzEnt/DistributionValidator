using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributionValidator.Models
{
  public sealed record DistributionStatus
  {
    public required string ClientId { get; init; }

    public required string Period { get; init; }

    public required string Status { get; init; }

    public required decimal DistributedAmount { get; init; }
  }
}
