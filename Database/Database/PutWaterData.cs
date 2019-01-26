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
    public static class PutWaterData
    {
        [FunctionName("PutWaterData")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                JObject waterData = JsonConvert.DeserializeObject<JObject>(requestBody);
                Uri serviceEndPoint = new Uri(Environment.GetEnvironmentVariable("CosmosEndPoint"));
                string key = Environment.GetEnvironmentVariable("CosmosKey");
                DocumentClient client = new DocumentClient(serviceEndPoint, key);
                var collectionUrl = UriFactory.CreateDocumentCollectionUri("streetworkout", "Data");
                FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true };
                string query = $"SELECT * FROM c WHERE c.Name = \"{waterData["Name"]}\" and c.Date = \"{waterData["Date"]}\" and c.Type = \"Water\"";
                JObject result = client.CreateDocumentQuery<JObject>(collectionUrl, query, queryOptions).AsEnumerable().FirstOrDefault();
                if (waterData["WaterGoal"] != null)
                {
                    result["WaterGoal"] = waterData["WaterGoal"];
                }
                if (waterData["WaterDrunk"] != null)
                {
                    result["WaterGeDrunk"] = waterData["WaterDrunk"];
                }
                var response = await client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri("streetworkout", "Data", result["id"].ToString()), result);
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