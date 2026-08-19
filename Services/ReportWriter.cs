using System.Globalization;
using System.Text;
using DistributionValidator.Models;

namespace DistributionValidator.Services;

public static class ReportWriter
{
  /// <summary>Writes one row per discrepancy, in a shape ready to drop straight into Excel.</summary>
  public static void WriteDetailCsv(string path, List<Discrepancy> discrepancies)
  {
    var sb = new StringBuilder();
    sb.AppendLine("client_id,client_name,discrepancy_type,source_amount,fee_pct,expected_net_amount,distributed_amount,difference,details");

    foreach (var d in discrepancies.OrderBy(d => d.Type).ThenBy(d => d.ClientId, StringComparer.Ordinal))
    {
      sb.AppendLine(string.Join(",",
          Csv(d.ClientId),
          Csv(d.ClientName),
          Csv(d.Type.ToString()),
          Csv(d.SourceAmount?.ToString("F2")),
          Csv(d.FeePct?.ToString("P0")),
          Csv(d.ExpectedNetAmount?.ToString("F2")),
          Csv(d.DistributedAmount?.ToString("F2")),
          Csv(d.Difference?.ToString("F2")),
          Csv(d.Details)));
    }

    File.WriteAllText(path, sb.ToString());
  }

  /// <summary>
  /// Writes a pivot-style summary: one row per discrepancy type, with the count and the
  /// affected client IDs, plus a grand total row. This is the CSV/text equivalent of the
  /// Excel pivot table requested in the assignment — see the .xlsx export for the actual
  /// pivot-formatted workbook.
  /// </summary>
  public static void WriteSummaryCsv(string path, List<Discrepancy> discrepancies)
  {
    var sb = new StringBuilder();
    sb.AppendLine("discrepancy_type,count,affected_client_ids");

    var groups = discrepancies
        .GroupBy(d => d.Type)
        .OrderByDescending(g => g.Count());

    foreach (var g in groups)
    {
      var clientIds = string.Join("; ", g.Select(d => d.ClientId).Distinct().OrderBy(id => id, StringComparer.Ordinal));
      sb.AppendLine(string.Join(",", Csv(g.Key.ToString()), g.Count(), Csv(clientIds)));
    }

    sb.AppendLine(string.Join(",", "TOTAL", discrepancies.Count,
        Csv(string.Join("; ", discrepancies.Select(d => d.ClientId).Distinct().OrderBy(id => id, StringComparer.Ordinal)))));

    File.WriteAllText(path, sb.ToString());
  }

  public static void PrintConsoleSummary(List<Discrepancy> discrepancies, int totalSourceClients, int totalOutputRows)
  {
    Console.WriteLine();
    Console.WriteLine("=== Distribution Validation Summary ===");
    Console.WriteLine($"Source clients checked : {totalSourceClients}");
    Console.WriteLine($"Output rows checked    : {totalOutputRows}");
    Console.WriteLine($"Discrepancies found    : {discrepancies.Count}");
    Console.WriteLine();
    Console.WriteLine($"{"Type",-24} {"Count",5}  Client IDs");
    Console.WriteLine(new string('-', 70));

    foreach (var g in discrepancies.GroupBy(d => d.Type).OrderByDescending(g => g.Count()))
    {
      var ids = string.Join(", ", g.Select(d => d.ClientId).Distinct().OrderBy(id => id, StringComparer.Ordinal));
      Console.WriteLine($"{g.Key,-24} {g.Count(),5}  {ids}");
    }
    Console.WriteLine();
  }

  private static string Csv(string? value)
  {
    value ??= string.Empty;
    return value.Contains(',') || value.Contains('"') || value.Contains('\n')
        ? "\"" + value.Replace("\"", "\"\"") + "\""
        : value;
  }
}
