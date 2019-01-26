using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.Documents.Client;
using System.Linq;
using System.Diagnostics;
using Microsoft.Azure.Documents;

namespace Database
{
    public static class DeleteWaterData
    {
        [FunctionName("DeleteWaterData")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "DeleteWaterData/{value}")] HttpRequest req, string value,
            ILogger log)
        {
            try
            {
                Uri serviceEndPoint = new Uri(Environment.GetEnvironmentVariable("CosmosEndPoint"));
                string key = Environment.GetEnvironmentVariable("CosmosKey");
                DocumentClient client = new DocumentClient(serviceEndPoint, key);
                var collectionUrl = UriFactory.CreateDocumentCollectionUri("streetworkout", "Data");
                FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true };
                string query = $"SELECT * FROM c WHERE c.Name = \"{value}\" and c.Type = \"Water\"";
                var result = client.CreateDocumentQuery<JObject>(collectionUrl, query, queryOptions).AsEnumerable();
                foreach (JObject data in result)
                {
                    await client.DeleteDocumentAsync(UriFactory.CreateDocumentUri("streetworkout", "Data", data["id"].ToString()), new RequestOptions { PartitionKey = new PartitionKey("Water") });
                }
                return new OkObjectResult(200);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
                throw ex;
            }
        }
    }
}
