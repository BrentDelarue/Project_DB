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

namespace Database
{
    //---------------------------------------------------------------------------------------//
    //----------------------------Ophalen van laatste Water data-----------------------------//
    //---------------------------------------------------------------------------------------//

    public static class GetLatestWaterData
    {
        [FunctionName("GetLatestWaterData")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetLatestWaterData/{value}")] HttpRequest req, string value,
            ILogger log)
        {
            try
            {
                //---Connectie met CosmosDB voorbereiden en maken---//
                Uri serviceEndPoint = new Uri(Environment.GetEnvironmentVariable("CosmosEndPoint"));
                string key = Environment.GetEnvironmentVariable("CosmosKey");
                DocumentClient client = new DocumentClient(serviceEndPoint, key);
                Uri collectionUrl = UriFactory.CreateDocumentCollectionUri("streetworkout", "Data");

                //---Query voorbereiden---//
                FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true };
                string query = $"SELECT * FROM c WHERE c.Name = \"{value}\" and c.Type = \"Water\" ORDER BY c.Date DESC";

                //---Ophalen van data uit CosmosDB en terug geven---//
                Water result = client.CreateDocumentQuery<Water>(collectionUrl, query, queryOptions).AsEnumerable().FirstOrDefault();
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
                throw ex;
            }
        }
    }
}
