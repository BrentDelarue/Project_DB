using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.Documents.Client;
using System.Linq;

namespace Database
{
    public static class CheckUserData
    {
        [FunctionName("CheckUserData")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                JObject userCheck = JsonConvert.DeserializeObject<JObject>(requestBody);
                Uri serviceEndPoint = new Uri(Environment.GetEnvironmentVariable("CosmosEndPoint"));
                string key = Environment.GetEnvironmentVariable("CosmosKey");
                DocumentClient client = new DocumentClient(serviceEndPoint, key);
                var collectionUrl = UriFactory.CreateDocumentCollectionUri("streetworkout", "Users");
                FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true };
                string query = $"SELECT * FROM c WHERE c.{userCheck["Check"]} = '{userCheck[userCheck["Check"].ToString()]}'";
                var result = client.CreateDocumentQuery<JObject>(collectionUrl, query, queryOptions).AsEnumerable();
                if (result.Count() > 0)
                {
                    return new OkObjectResult(true);
                }
                else
                {
                    return new OkObjectResult(false);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
                throw ex;
            }
        }
    }
}
