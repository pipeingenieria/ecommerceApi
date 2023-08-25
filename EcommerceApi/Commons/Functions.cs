using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Threading.Tasks;

namespace eCommerce.Commons
{
    public static class Functions
    {
        public static string getConfig(string key)
        {
            return Environment.GetEnvironmentVariable(key);
        }

        public static async Task UpdateTable(CloudTable table, ITableEntity entity)
        {
            //// operación de actualización
            TableOperation updateOperation = TableOperation.InsertOrMerge(entity);
            await table.ExecuteAsync(updateOperation);
        }
    }
}
