namespace DistributionValidator.Models;

/// <summary>
/// Categories of discrepancy the validator can detect between source_calculations.csv
/// data and distribution_output.csv data.
/// </summary>
public enum DiscrepancyType
{
  /// <summary>Client exists in source calculations but has no distribution output record at all.</summary>
  MissingInOutput,

  /// <summary>Client exists in distribution output but has no corresponding source calculation record.</summary>
  UnexpectedInOutput,

  /// <summary>Client has more than one distribution output record for the same run (potential double-payment).</summary>
  DuplicateOutputRecord,

  /// <summary>Client has more than one source calculation record for the same run.</summary>
  DuplicateSourceRecord,

  /// <summary>The source file's own "expected net amount" does not match source_amount * (1 - fee_pct).
  /// This flags a data-integrity problem upstream of distribution, not a distribution bug.</summary>
  SourceFormulaMismatch,

  /// <summary>Distributed amount differs from expected net amount by a cent or less — consistent with a
  /// rounding/precision issue rather than a broken calculation.</summary>
  RoundingDiscrepancy,

  /// <summary>Distributed amount differs from expected net amount by more than a cent — a real calculation error.</summary>
  CalculationError
}
