# Task 1. Distribution Validator

A .NET 8 console tool that validates bulk financial distribution output against source calculation data, at the individual client level, down to the cent, and categorizes every discrepancy found.

## Build Solution

```bash
# From inside the project folder:
dotnet build
```

## Run Solution

```bash
# From inside the project folder:
dotnet run
```

By default it reads `Data/source_calculations.csv` and `Data/distribution_output.csv` (the sample files from the assignment) and writes reports to `Output` folder.
You can point it to a custom larger dataset that I created to test how the system would scale to larger datasets.

```bash
dotnet run -- Data/source_calculations_50K.csv Data/distribution_output_50k.csv
```

## Output

- `Output/discrepancies_detail.csv`: one row per discrepancy, with amounts and a human-readable explanation. Opens directly in Excel.
- `Output/discrepancy_summary.csv`: one row per discrepancy type, with count and affected client IDs.
- Console output: the same summary printed to stdout for quick review.

# Task 2. SQL Investigation & Root Cause Analysis
Connect to `Database/distribution_qa.db` using SqLite

- Queries related to the tasks are written in [Task2.sql](Database/Task2.sql)
- Bug Report and RCA can be found in [BugReport](BugReport/BugReport.docx)


# Task 3. Automated Regression Coverage

## Build Solution

```bash
# From inside the project folder:
dotnet build
```

## Run Tests

```bash
# From inside the project folder:
dotnet test
```
Database call is mocked out using NSubstitute.
Tests positive and negative cases:
- 3 positive: COMPLETED, PENDING, and FAILED distributions each return the correct status and amount.
- 6 negative/edge: empty client ID, empty period, and three malformed period formats.

See [SOLUTION.md](SOLUTION.md) for the regression strategy behind this scope (why these scenarios,
prioritized for a two-day release window, and what was deliberately left out).