using DistributionValidator.Services;
using System;

var sourcePath = args.Length > 0 ? args[0] : Path.Combine("Data", "source_calculations.csv");
var outputPath = args.Length > 1 ? args[1] : Path.Combine("Data", "distribution_output.csv");
var reportDir = args.Length > 2 ? args[2] : "Output";

if (!ValidateFile(sourcePath, "Source") || !ValidateFile(outputPath, "Output"))
{
  return 1;
}

Directory.CreateDirectory(reportDir);

var sourceRecords = DataLoader.LoadSourceCalculations(sourcePath);
var outputRecords = DataLoader.LoadDistributionOutput(outputPath);

var engine = new DistributionValidatorEngine();
var discrepancies = DistributionValidatorEngine.Validate(sourceRecords, outputRecords);

var detailPath = Path.Combine(reportDir, "discrepancies_detail.csv");
var summaryPath = Path.Combine(reportDir, "discrepancy_summary.csv");

//Write to files
ReportWriter.WriteDetailCsv(detailPath, discrepancies);
ReportWriter.WriteSummaryCsv(summaryPath, discrepancies);

//Write to console
ReportWriter.PrintConsoleSummary(discrepancies, sourceRecords.Select(s => s.ClientId).Distinct().Count(), outputRecords.Count);
Console.WriteLine($"Detail report  -> {detailPath}");
Console.WriteLine($"Summary report -> {summaryPath}");
Console.WriteLine();

return 0;

static bool ValidateFile(string filePath, string fileType)
{
  if (!Path.GetExtension(filePath).Equals(".csv", StringComparison.OrdinalIgnoreCase))
  {
    Console.Error.WriteLine($"Invalid file type for {fileType}: {filePath}. Only .csv files are allowed.");
    return false;
  }
  if (!File.Exists(filePath))
  {
    Console.Error.WriteLine($"{fileType} file not found: {filePath}");
    return false;
  }
  return true;
}
