using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using eCommerce.Commons;
using System.Net.Http;
using Newtonsoft.Json;

namespace eCommerce.Shared
{
    class Table
    {
        public static CloudTable GetTable(string tableName)
        {
            //Conexión con el azure storage
            CloudStorageAccount storageAccount = Authentication.authAccountStorage();

            var client = storageAccount.CreateCloudTableClient();
            // obtenemos la referencia a la tabla
            return client.GetTableReference(tableName);
        }

        public static T GetProductBody<T>(HttpRequestMessage req)
        {
            var content = req.Content;
            string contentString = content.ReadAsStringAsync().Result;
            T entity = JsonConvert.DeserializeObject<T>(contentString);
            return entity;
        }
    }
}
