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

namespace Database
{
    public static class GetUserName
    {
        [FunctionName("GetUserName")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Gebruiker user = JsonConvert.DeserializeObject<Gebruiker>(requestBody);
            Uri serviceEndPoint = new Uri(Environment.GetEnvironmentVariable("CosmosEndPoint"));
            string key = Environment.GetEnvironmentVariable("CosmosKey");
            DocumentClient client = new DocumentClient(serviceEndPoint, key);
            var collectionUrl = UriFactory.CreateDocumentCollectionUri("streetworkout", "Users");
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true };
            string query = $"SELECT * FROM c WHERE c.Email = \"{user.Email}\"";
            var result = client.CreateDocumentQuery<JObject>(collectionUrl, query, queryOptions).AsEnumerable().FirstOrDefault();
            return new OkObjectResult(result["Naam"].ToString());
        }
    }
}
