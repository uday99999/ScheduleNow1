using CsvHelper;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;

public static class CsvHelper1
{
    public static async Task WriteRecordsToCsvAsync(IEnumerable<dynamic> records, string filePath, ILogger logger)
    {
        logger.LogInformation($"Writing records to CSV file: {filePath}");

        using (var writer = new StreamWriter(filePath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
           await csv.WriteRecordsAsync(records);
        }
        logger.LogInformation("Writing to CSV file completed.");
    }
}