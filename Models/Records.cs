namespace DistributionValidator.Models;

/// <summary>One row from source_calculations.csv.</summary>
public sealed record SourceRecord
{
  public required string ClientId { get; init; }
  public required string ClientName { get; init; }
  public required decimal SourceAmount { get; init; }
  public required decimal FeePct { get; init; }
  public required decimal ExpectedNetAmount { get; init; }
}

/// <summary>One row from distribution_output.csv.</summary>
public sealed record OutputRecord
{
  public required string ClientId { get; init; }
  public required string ClientName { get; init; }
  public required decimal DistributedAmount { get; init; }
  public required string Status { get; init; }
}

/// <summary>A single detected discrepancy, ready to be written to the report.</summary>
public sealed record Discrepancy
{
  public required string ClientId { get; init; }
  public required string ClientName { get; init; }
  public required DiscrepancyType Type { get; init; }
  public decimal? SourceAmount { get; init; }
  public decimal? FeePct { get; init; }
  public decimal? ExpectedNetAmount { get; init; }
  public decimal? DistributedAmount { get; init; }
  public decimal? Difference { get; init; }
  public required string Details { get; init; }
}
