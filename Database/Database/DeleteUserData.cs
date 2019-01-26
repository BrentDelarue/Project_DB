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
using System.Reflection.Metadata;
using System.Collections.ObjectModel;
using Microsoft.Azure.Documents;

namespace Database
{
    public static class DeleteUserData
    {
        [FunctionName("DeleteUserData")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "DeleteUserData/{value}")] HttpRequest req, string value,
            ILogger log)
        {
            try
            {
                Uri serviceEndPoint = new Uri(Environment.GetEnvironmentVariable("CosmosEndPoint"));
                string key = Environment.GetEnvironmentVariable("CosmosKey");
                DocumentClient client = new DocumentClient(serviceEndPoint, key);
                var collectionUrl = UriFactory.CreateDocumentCollectionUri("streetworkout", "Data");
                FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true };
                string query = $"SELECT * FROM c WHERE c.Name = \"{value}\" and c.Type = \"User\"";
                var result = client.CreateDocumentQuery<JObject>(collectionUrl, query, queryOptions).AsEnumerable().FirstOrDefault();
                var response = await client.DeleteDocumentAsync(UriFactory.CreateDocumentUri("streetworkout", "Data", result["id"].ToString()), new RequestOptions { PartitionKey = new PartitionKey("User") });
                return new OkObjectResult(result["Type"].ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
                throw ex;
            }
        }
    }
}
