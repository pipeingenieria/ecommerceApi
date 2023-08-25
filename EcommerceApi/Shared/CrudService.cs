using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using eCommerce.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.Shared
{
    class CrudService
    {
        public static async Task<T> InsertOrMergeEntityAsync<T>(CloudTable table, ITableEntity entity, TraceWriter log)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            try
            {
                // Create the InsertOrReplace table operation
                TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);

                // Execute the operation.
                TableResult result = await table.ExecuteAsync(insertOrMergeOperation);
                T insertedCustomer = (T)result.Result;

                return insertedCustomer;
            }
            catch (StorageException e)
            {
                log.Error(e.Message);
                throw;
            }
        }

        public static async Task DeleteEntityAsync(CloudTable table, ITableEntity deleteEntity, TraceWriter log)
        {
            try
            {
                if (deleteEntity == null)
                {
                    throw new ArgumentNullException("deleteEntity");
                }

                TableOperation deleteOperation = TableOperation.Delete(deleteEntity);
                TableResult result = await table.ExecuteAsync(deleteOperation);

            }
            catch (StorageException e)
            {
                log.Error("Crud services - delete: " + e.Message);
                throw;
            }
        }

    }
}
