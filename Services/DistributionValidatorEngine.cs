using DistributionValidator.Models;

namespace DistributionValidator.Services;

/// <summary>
/// Validates distribution output against source calculation data at the individual
/// client level, to the cent, and categorizes every discrepancy found.
/// </summary>
public sealed class DistributionValidatorEngine
{
  /// <summary>Anything within this many dollars is treated as a rounding artifact rather than a calculation error.</summary>
  private const decimal RoundingTolerance = 0.01m;

  public static List<Discrepancy> Validate(List<SourceRecord> sourceRecords, List<OutputRecord> outputRecords)
  {
    var discrepancies = new List<Discrepancy>();

    var sourceByClient = sourceRecords.GroupBy(r => r.ClientId).ToDictionary(g => g.Key, g => g.ToList());
    var outputByClient = outputRecords.GroupBy(r => r.ClientId).ToDictionary(g => g.Key, g => g.ToList());

    var allClientIds = sourceByClient.Keys
        .Union(outputByClient.Keys)
        .OrderBy(id => id, StringComparer.Ordinal);

    foreach (var clientId in allClientIds)
    {
      sourceByClient.TryGetValue(clientId, out var sources);
      outputByClient.TryGetValue(clientId, out var outputs);

      var hasSource = sources is { Count: > 0 };
      var hasOutput = outputs is { Count: > 0 };

      // --- Missing / orphan records -------------------------------------------------
      if (hasSource && !hasOutput)
      {
        var s = sources![0];
        discrepancies.Add(new Discrepancy
        {
          ClientId = clientId,
          ClientName = s.ClientName,
          Type = DiscrepancyType.MissingInOutput,
          SourceAmount = s.SourceAmount,
          FeePct = s.FeePct,
          ExpectedNetAmount = s.ExpectedNetAmount,
          DistributedAmount = null,
          Difference = null,
          Details = $"Client present in source_calculations.csv (expected {s.ExpectedNetAmount:F2}) " +
                      "but has no row at all in distribution_output.csv. Client was never paid."
        });
        continue;
      }

      if (hasOutput && !hasSource)
      {
        foreach (var o in outputs!)
        {
          discrepancies.Add(new Discrepancy
          {
            ClientId = clientId,
            ClientName = o.ClientName,
            Type = DiscrepancyType.UnexpectedInOutput,
            SourceAmount = null,
            FeePct = null,
            ExpectedNetAmount = null,
            DistributedAmount = o.DistributedAmount,
            Difference = null,
            Details = $"Client has a distribution_output.csv row (status={o.Status}, amount={o.DistributedAmount:F2}) " +
                        "with no matching client in source_calculations.csv. No basis to justify this payment."
          });
        }
        continue;
      }

      // --- Both sides present: check for duplicates -----------------------------------
      if (sources!.Count > 1)
      {
        discrepancies.Add(new Discrepancy
        {
          ClientId = clientId,
          ClientName = sources[0].ClientName,
          Type = DiscrepancyType.DuplicateSourceRecord,
          SourceAmount = sources[0].SourceAmount,
          Details = $"Client appears {sources.Count} times in source_calculations.csv for this run."
        });
      }

      if (outputs!.Count > 1)
      {
        discrepancies.Add(new Discrepancy
        {
          ClientId = clientId,
          ClientName = outputs[0].ClientName,
          Type = DiscrepancyType.DuplicateOutputRecord,
          DistributedAmount = outputs[0].DistributedAmount,
          Details = $"Client appears {outputs.Count} times in distribution_output.csv for this run " +
                      $"(total if all paid: {outputs.Sum(o => o.DistributedAmount):F2}). Risk of double-payment."
        });
      }

      // --- Amount validation (use the first source row as authoritative for the run) --
      var source = sources[0];

      // Cross-check the source file's own arithmetic: source_amount * (1 - fee_pct)
      // should equal expected_net_amount. If it doesn't, the problem predates
      // distribution entirely and developers need to know that distinction.
      var recomputedExpected = decimal.Round(
          source.SourceAmount * (1 - source.FeePct), 2, MidpointRounding.AwayFromZero);

      if (recomputedExpected != source.ExpectedNetAmount)
      {
        discrepancies.Add(new Discrepancy
        {
          ClientId = clientId,
          ClientName = source.ClientName,
          Type = DiscrepancyType.SourceFormulaMismatch,
          SourceAmount = source.SourceAmount,
          FeePct = source.FeePct,
          ExpectedNetAmount = source.ExpectedNetAmount,
          Difference = recomputedExpected - source.ExpectedNetAmount,
          Details = $"source_amount ({source.SourceAmount:F2}) * (1 - fee_pct {source.FeePct:P0}) = " +
                      $"{recomputedExpected:F2}, but source file states expected_net_amount = " +
                      $"{source.ExpectedNetAmount:F2}. Source data itself is inconsistent."
        });
      }

      foreach (var output in outputs)
      {
        var difference = output.DistributedAmount - source.ExpectedNetAmount;

        if (difference == 0m)
        {
          continue; // matches to the cent — no discrepancy
        }

        var absDiff = Math.Abs(difference);
        var type = absDiff <= RoundingTolerance
            ? DiscrepancyType.RoundingDiscrepancy
            : DiscrepancyType.CalculationError;

        var note = type == DiscrepancyType.RoundingDiscrepancy
            ? "Off by a cent or less — consistent with a rounding/precision bug, not a broken formula."
            : "Off by more than a cent — expected amount does not reconcile with what was paid.";

        discrepancies.Add(new Discrepancy
        {
          ClientId = clientId,
          ClientName = source.ClientName,
          Type = type,
          SourceAmount = source.SourceAmount,
          FeePct = source.FeePct,
          ExpectedNetAmount = source.ExpectedNetAmount,
          DistributedAmount = output.DistributedAmount,
          Difference = difference,
          Details = $"Expected {source.ExpectedNetAmount:F2}, distributed {output.DistributedAmount:F2} " +
                      $"(diff {difference:+0.00;-0.00}). {note}"
        });
      }
    }

    return discrepancies;
  }
}
