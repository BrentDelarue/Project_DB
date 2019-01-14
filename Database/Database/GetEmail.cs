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

namespace Database
{
    public static class GetEmail
    {
        [FunctionName("GetEmail")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            JObject user = JsonConvert.DeserializeObject<JObject>(requestBody);
            Uri serviceEndPoint = new Uri(Environment.GetEnvironmentVariable("CosmosEndPoint"));
            string key = Environment.GetEnvironmentVariable("CosmosKey");
            DocumentClient client = new DocumentClient(serviceEndPoint, key);
            var collectionUrl = UriFactory.CreateDocumentCollectionUri("streetworkout", "Users");
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true };
            string query = $"SELECT * FROM c WHERE c.Naam = \"{user["Naam"]}\"";
            var result = client.CreateDocumentQuery<JObject>(collectionUrl, query, queryOptions).AsEnumerable().FirstOrDefault();
            return new OkObjectResult(result["Email"].ToString());
        }
    }
}
