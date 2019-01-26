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
using System.Diagnostics;
using System.Linq;
using Microsoft.Azure.Documents;

namespace Database
{
    public static class DeleteExerciseData
    {
        [FunctionName("DeleteExerciseData")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "DeleteExerciseData/{value}")] HttpRequest req, string value,
            ILogger log)
        {
            try
            {
                Uri serviceEndPoint = new Uri(Environment.GetEnvironmentVariable("CosmosEndPoint"));
                string key = Environment.GetEnvironmentVariable("CosmosKey");
                DocumentClient client = new DocumentClient(serviceEndPoint, key);
                var collectionUrl = UriFactory.CreateDocumentCollectionUri("streetworkout", "Data");
                FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true };
                string query = $"SELECT * FROM c WHERE c.Name = \"{value}\" and c.Type = \"Exercise\"";
                var result = client.CreateDocumentQuery<JObject>(collectionUrl, query, queryOptions).AsEnumerable();
                foreach (JObject oefening in result)
                {
                    await client.DeleteDocumentAsync(UriFactory.CreateDocumentUri("streetworkout", "Data", oefening["id"].ToString()), new RequestOptions { PartitionKey = new PartitionKey("Exercise") });
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
