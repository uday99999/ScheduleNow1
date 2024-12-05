using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ScheduleNow1
{
    public static class DatabaseHelper
    {
        public static async Task<IEnumerable<dynamic>> GetDataFromTable(string tableName, IConfiguration configuration, ILogger logger)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            logger.LogInformation($"Fetching data from database");

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var data = await db.QueryAsync("GetTableData", commandType: CommandType.StoredProcedure);
                logger.LogInformation($"Fetched {data.AsList().Count} records from database");
                return data;

            }
        }
    }
}