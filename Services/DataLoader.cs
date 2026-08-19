using System.Globalization;
using DistributionValidator.Models;

namespace DistributionValidator.Services;

public static class DataLoader
{
  public static List<SourceRecord> LoadSourceCalculations(string path)
  {
    var rows = CsvReader.ReadRecords(path);
    var records = new List<SourceRecord>(rows.Count);

    foreach (var row in rows)
    {
      records.Add(new SourceRecord
      {
        ClientId = row["client_id"].Trim(),
        ClientName = row["client_name"].Trim(),
        SourceAmount = ParseDecimal(row["source_amount"]),
        FeePct = ParseDecimal(row["fee_pct"]),
        ExpectedNetAmount = ParseDecimal(row["expected_net_amount"])
      });
    }

    return records;
  }

  public static List<OutputRecord> LoadDistributionOutput(string path)
  {
    var rows = CsvReader.ReadRecords(path);
    var records = new List<OutputRecord>(rows.Count);

    foreach (var row in rows)
    {
      records.Add(new OutputRecord
      {
        ClientId = row["client_id"].Trim(),
        ClientName = row["client_name"].Trim(),
        DistributedAmount = ParseDecimal(row["distributed_amount"]),
        Status = row["status"].Trim()
      });
    }

    return records;
  }

  /// <summary>
  /// Parses money/percent fields as `decimal` (base-10, exact) rather than `double`.
  /// Using `double` here would reintroduce binary floating-point rounding error into
  /// a system whose entire purpose is validating amounts to the cent.
  /// </summary>
  private static decimal ParseDecimal(string raw)
  {
    var cleaned = raw.Trim().Replace(",", string.Empty).Replace("%", string.Empty);
    if (string.IsNullOrWhiteSpace(cleaned))
    {
      return 0m;
    }
    return decimal.Parse(cleaned, NumberStyles.Number | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture);
  }
}
