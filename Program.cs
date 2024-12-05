using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ScheduleNow1;
using Serilog;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        const string tableName = "dbo.products";
        const string sftpHost = "your.sftp.server";
        const string sftpUsername = "yourUsername";
        const string sftpPassword = "yourPassword";
        const string remoteDirectory = "/remote/path";

        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        IConfiguration configuration = builder.Build();

        // Configure Serilog for logging
        Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .ReadFrom.Configuration(configuration)
                    .WriteTo.File(string.Format(configuration.GetSection("LogFilePathMask").Value, DateTime.Now),
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss:fff} {Message}{NewLine}{Exception}")
                    .CreateLogger();

        

        // Set up logging
        var serviceProvider = new ServiceCollection()
            .AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders(); // Clear other logging providers
                loggingBuilder.AddSerilog(dispose: true); // Add Serilog as the logging provider
            })
            .BuildServiceProvider();

        var logger = serviceProvider.GetService<ILogger<Program>>();

        // Read SFTP settings from configuration
        //var sftpSettings = configuration.GetSection("SftpSettings").Get<SftpSettings>();


        // Read connection string and SFTP settings from configuration
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var csvFilePath = configuration["FilePaths:CsvFilePath"];

        var currentDate = DateTime.Now.ToString("yyyyMMdd"); // Customize format as needed
        var csvFileName = string.Format(csvFilePath, currentDate);

        try
        {
            logger!.LogInformation("Task started.");

            var data = await DatabaseHelper.GetDataFromTable(tableName, configuration, logger);
            await CsvHelper1.WriteRecordsToCsvAsync(data, csvFileName, logger);
            //await SftpHelper.UploadFile(csvFilePath, sftpSettings.Host, sftpSettings.Username, sftpSettings.Password, sftpSettings.RemoteDirectory, logger);

            logger.LogInformation("Task completed.\n------------------------------");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"An error occurred during task execution.");
            logger.LogError("\n----------------------------");
        }
        finally
        {
            Log.CloseAndFlush(); // Ensure all logs are flushed and closed properly
        }

    }
}