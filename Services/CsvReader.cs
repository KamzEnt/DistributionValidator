using System.Globalization;
using CsvHelper.Configuration;

namespace DistributionValidator.Services;

/// <summary>
/// CSV reader using CsvHelper library. Handles quoted fields, embedded commas,
/// embedded quotes and various line endings.
/// </summary>
public static class CsvReader
{
  /// <summary>Reads a CSV file and returns each row as a header-name -> value dictionary.</summary>
  public static List<Dictionary<string, string>> ReadRecords(string path)
  {
    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
    {
      HeaderValidated = null,
      MissingFieldFound = null
    };

    using var reader = new StreamReader(path);
    using var csv = new CsvHelper.CsvReader(reader, config);
    
    var records = new List<Dictionary<string, string>>();
    csv.Read();
    csv.ReadHeader();
    
    while (csv.Read())
    {
      var record = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
      foreach (var header in csv.HeaderRecord!)
      {
        record[header] = csv.GetField(header) ?? string.Empty;
      }
      records.Add(record);
    }

    return records;
  }
}
