using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;
using eCommerce.Commons;
using eCommerce.EntityModels;
using Newtonsoft.Json;
using eCommerce.Shared;
using Microsoft.WindowsAzure.Storage;
using System.Collections.Generic;

namespace eCommerce.DataAcces
{
    public static class Products
    {
        // Indica la tabla con la que se trabajar�
        private const string _table = "ListConfiguration";

        // Indica la partici�n con la que se trabajar�
        private const string _partitionKey = "Product";




        [FunctionName("GetProducts")]
        public static async Task<HttpResponseMessage> GetProducts([HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "Product")] HttpRequestMessage req, TraceWriter log)
        {
            log.Info("GET ALL -> getting all the Products.");
            try
            {
                // obteniendo la referencia de la tabla
                CloudTable table = Table.GetTable(_table);
                ITableEntity Product = Table.GetProductBody<ProductsEntity>(req);

                // Consulta sincrona desactivada
                //List<ProductsEntity> _Product = table.CreateQuery<ProductsEntity>().Where(x => x.PartitionKey == _partitionKey).ToList();

                // modificaciones asincronas
                TableQuery<ProductsEntity> query = new TableQuery<ProductsEntity>()
                    .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, _partitionKey));

                TableContinuationToken token = null;
                List<ProductsEntity> _Product = new List<ProductsEntity>();
                do
                {
                    TableQuerySegment<ProductsEntity> tableSegment = await table.ExecuteQuerySegmentedAsync(query, token);
                    _Product.AddRange(tableSegment.ToList());
                    token = tableSegment.ContinuationToken;
                } while (token != null);

                // fin modificaciones asincronas


                // Envio a Aplication Insigths
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.Context.User.Id = "...";
                telemetry.Context.Device.Id = "...";
                telemetry.TrackEvent(_Product.ToList().ToString());

                return req.CreateResponse(HttpStatusCode.OK, _Product.ToList(), "application/json");
            }
            catch (Exception ex)
            {
                // Control de excepciones
                log.Error(string.Format("GET ALL -> error during the process of getting Products: {0}", ex.ToString()));

                // Envio a Aplication Insigths
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);

                //Retorno del Request
                return req.CreateResponse(HttpStatusCode.BadRequest, string.Format("There was the following error when obtaining the records from the Products list: {0} ", ex.ToString()));
            }
        }


        [FunctionName("GetProduct")]
        public static async Task<HttpResponseMessage> GetProduct([HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "Product/{RowKey}")] HttpRequestMessage req,
            string RowKey, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request on GetProduct.");

            try
            {
                log.Info("Getting Product...");
                CloudTable table = Table.GetTable(_table);
                ITableEntity Product = Table.GetProductBody<ProductsEntity>(req);
                //ProductsEntity _Product = table.CreateQuery<ProductsEntity>().Where(x => x.PartitionKey == "Product" && x.RowKey == RowKey).FirstOrDefault();

                TableOperation insertOrMergeOperation = TableOperation.Retrieve(_partitionKey, RowKey);

                TableResult result = await table.ExecuteAsync(insertOrMergeOperation);
                ProductsEntity _Product = result.Result as ProductsEntity;

                _Product = await RetrieveEntityUsingPointQueryAsync(table, _Product.PartitionKey, _Product.RowKey, log);

                // Envio a Aplication Insigths
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackEvent(_Product.ToString());

                return _Product == null
                ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a parameter valid on the query string or in the request url. The item has no found")
                : req.CreateResponse(HttpStatusCode.OK, _Product);
            }
            catch (Exception ex)
            {
                var error = $"Getting Product has failed: {ex.Message}";
                log.Error(error);

                // Envio a Aplication Insigths
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);

                return req.CreateResponse(HttpStatusCode.BadRequest, error);
            }
        }

        [FunctionName("CreateProduct")]
        public static async Task<HttpResponseMessage> CreateProduct([HttpTrigger(AuthorizationLevel.Anonymous, "POST", Route = "Product")] HttpRequestMessage req,
            TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request on CreateProduct.");

            try
            {
                log.Info("Creating Product...");

                CloudTable table = Table.GetTable(_table);
                ITableEntity Product = Table.GetProductBody<ProductsEntity>(req);
                await CrudService.InsertOrMergeEntityAsync(table, Product, log);

                // Envio a Aplication Insigths
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackEvent(Product.ToString());

                return req.CreateResponse(HttpStatusCode.OK, Product);

            }
            catch (Exception ex)
            {
                var error = $"CreateProduct failed: {ex.Message}";
                log.Error(error);

                // Envio a Aplication Insigths
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);

                return req.CreateResponse(HttpStatusCode.BadRequest, error);
            }
        }

        //Filtrar Producta por Usuario
        [FunctionName("MyProduct")]
        public static async Task<HttpResponseMessage> MyProduct([HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "Product/{EmailResponsible}")] HttpRequestMessage req,
             string EmailResponsible, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request on GetProduct.");

            try
            {
                log.Info("Getting Product...");
                CloudTable table = Table.GetTable(_table);

                ITableEntity Product = Table.GetProductBody<ProductsEntity>(req);
                var query = new TableQuery<ProductsEntity>().Where(TableQuery.CombineFilters(
                                                            TableQuery.GenerateFilterCondition(nameof(TableEntity.PartitionKey), QueryComparisons.Equal, _partitionKey),
                                                            TableOperators.And,
                                                            TableQuery.GenerateFilterCondition("EmailResponsible", QueryComparisons.Equal, EmailResponsible)));
                
                List<ProductsEntity> _Product = table.ExecuteQuerySegmentedAsync(query, null).Result.ToList();

                // Envio a Aplication Insigths
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackEvent(_Product.ToString());

                return _Product == null
                ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a parameter valid on the query string or in the request url. The item has no found")
                : req.CreateResponse(HttpStatusCode.OK, _Product);
            }
            catch (Exception ex)
            {
                var error = $"Getting Product has failed: {ex.Message}";
                log.Error(error);

                // Envio a Aplication Insigths
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);

                return req.CreateResponse(HttpStatusCode.BadRequest, error);
            }
        }

        [FunctionName("DeleteProduct")]
        public static async Task<HttpResponseMessage> DeleteProduct([HttpTrigger(AuthorizationLevel.Anonymous, "PUT", Route = "Product")] HttpRequestMessage req,
            TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request on DeleteProduct.");

            try
            {
                //KeyModel key = GetKeys(req);

                CloudTable table = Table.GetTable(_table);
                dynamic body = await req.Content.ReadAsStringAsync();
                ProductsEntity record = JsonConvert.DeserializeObject<ProductsEntity>(body as string);
                ProductsEntity Product = await RetrieveEntityUsingPointQueryAsync(table, record.PartitionKey, record.RowKey, log);
                await CrudService.DeleteEntityAsync(table, Product, log);

                // Envio a Aplication Insigths
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackEvent(Product.ToString());

                return req.CreateResponse(HttpStatusCode.OK, "Deleted OK");

            }
            catch (Exception ex)
            {
                var error = $"DeleteProduct failed: {ex.Message}";
                log.Error(error);

                // Envio a Aplication Insigths
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);

                return req.CreateResponse(HttpStatusCode.BadRequest, error);
            }
        }

        public static async Task<ProductsEntity> RetrieveEntityUsingPointQueryAsync(CloudTable table, string partitionKey, string rowKey, TraceWriter log)
        {
            try
            {
                TableOperation retrieveOperation = TableOperation.Retrieve<ProductsEntity>(partitionKey, rowKey);
                TableResult result = await table.ExecuteAsync(retrieveOperation);
                ProductsEntity entity = result.Result as ProductsEntity;
                if (entity == null)
                {
                    throw new ArgumentNullException("PartitionKey and/or RowKey");
                }

                return entity;
            }
            catch (StorageException ex)
            {
                log.Error(ex.Message);

                // Envio a Aplication Insigths
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);

                throw;
            }
        }

    }
}
