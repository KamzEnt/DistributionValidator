# Task 1: Distribution Validator System
This system will compare a source and distribution csv files, filter out any discrepancies between the files and populate them into a detailed and summarised csv files.

The system is split into the following classes to enforce maintainablity and modularity. (Thinking along the lines of Single Responsibility Principle)
Listed in order of flow:
- CsvReader.cs: reads data from csv files, performs data cleanup (quoted fields, embedded commas, line endings etc) by using CsvHelper NuGet package.
- DataLoader.cs: Create a List from the data passed in by CsvReader, this list will be processed in DistributionValidatorEngine.cs
- DistributionValidatorEngine.cs: The distribution data is compared against the source data, any discrepancies found will be added to a Discrepancies list.
- ReportWriter.cs: The Discrepancies list is passed here, sorted by type, and populated into a detailed and summary csv reports.

## Design Considerations
- Built as a .Net 8 console application, to be a light, easy to setup and run application.
- Used CsvHelper NuGet package to avoid re-creating functionality that exists.
- Used StreamReader over File.ReadAllText() which is more memory efficient, StreamReader can handle larger files (useful when we don't know the size of dataset we are working with.) Have added additional datasets to test this (source_calculations_50k.csv, distribution_output_50K.csv)


# Task 2: SQL Investigation & Root Cause Analysis
- Queries related to the tasks are written in [Task2.sql](Database/Task2.sql)
- Bug Report and RCA can be found in [BugReport](BugReport/BugReport.docx)



# Task 3: Automated Regression Coverage
## Regression Strategy
- Objective
  -  Provide confidence in the product release, given a two-day release timeline and a limited testing window. Testing will prioritize business risk.
- Approach
  - Due to time constraints, automated tests will cover the in scope scenarios to ensure fast feedback on the system.
- In Scope
  - Core system flow only (All valid/invalid ClientID's and Periods).
  - I would want to confirm that the system is able to perform its designed function.
- Out of Scope
  - Exploratory testing will not be performed.
  - Every client and period combination.
  - Manual testing due to time constraints.
- Risks
  - Failover scenario's not tested.
  - Large volume data set testing.




# AI Disclaimer
I used Claude (Anthropic) throughout this assignment:

- Task 1:
  - Scaffold the initial C# project structure (CSV parsing, the validation engine, and report writer), and to generate the 50,000-row synthetic dataset used for the scaling sanity check.
  - Verified by building and running the project myself and checking the output against the known sample-data discrepancies.
- Task 2: 
  - Sanity-check my SQL against the actual database, I'd write a query, ask it to run it against distribution_qa.db and explain any discrepancy between what I expected and what came back.
  - Verified by running every query myself against the real database and checking the returned rows.
- Task 3: 
  - Help scaffold the NUnit/NSubstitute test project structure.
  - Verified by running the test suite myself and confirming all tests pass.
