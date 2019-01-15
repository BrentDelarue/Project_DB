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
using System.Collections.Generic;
using System.Diagnostics;

namespace Database
{
    public static class APIUpdate
    {
        [FunctionName("APIUpdate")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                JObject user = JsonConvert.DeserializeObject<JObject>(requestBody);
                Uri serviceEndPoint = new Uri(Environment.GetEnvironmentVariable("CosmosEndPoint"));
                string key = Environment.GetEnvironmentVariable("CosmosKey");
                DocumentClient client = new DocumentClient(serviceEndPoint, key);
                var collectionUrl = UriFactory.CreateDocumentCollectionUri("streetworkout", "Users");
                FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true };
                string query = $"SELECT * FROM c WHERE c.Naam = \"{user["Naam"]}\"";
                JObject result = client.CreateDocumentQuery<JObject>(collectionUrl, query, queryOptions).AsEnumerable().FirstOrDefault();
                if (user["Gewicht"] != null)
                {
                    result["Gewicht"] = user["Gewicht"];
                }
                if (user["Lengte"] != null)
                {
                    result["Lengte"] = user["Lengte"];
                }
                if (user["Leeftijd"] != null)
                {
                    result["Leeftijd"] = user["Leeftijd"];
                }
                if (user["API"] != null)
                {
                    result["API"] = user["API"];
                }
                if (user["WaterDoel"] != null)
                {
                    result["WaterDoel"] = user["WaterDoel"];
                }
                //if (user["Achievements"].Count() != 0)
                //{
                //    var lijstje = JArray.Parse(result["Achievements"].ToList());
                //    foreach (string achievement in user["Achievements"])
                //    {
                //        lijstje.Add(achievement);
                //    }
                //    result["Achievements"] = (JArray)lijstje;
                //}
                var response = await client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri("streetworkout", "Users", result["id"].ToString()), result);
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
