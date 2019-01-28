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
    //------------------------------Checken van Login gegevens-------------------------------//
    //---------------------------------------------------------------------------------------//

    public static class Login
    {
        [FunctionName("Login")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Login/{name}/{password}")] HttpRequest req, string name, string password,
            ILogger log)
        {
            try
            {
                //---Connectie met CosmosDB voorbereiden en maken---//
                Uri serviceEndPoint = new Uri(Environment.GetEnvironmentVariable("CosmosEndPoint"));
                string key = Environment.GetEnvironmentVariable("CosmosKey");
                DocumentClient client = new DocumentClient(serviceEndPoint, key);
                var collectionUrl = UriFactory.CreateDocumentCollectionUri("streetworkout", "Data");

                //---Query voorbereiden---//
                FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true };
                string query = $"SELECT * FROM c WHERE c.Name = \"{name}\" and c.Password = \"{password}\" and c.Type = \"User\"";

                //---Ophalen van data uit CosmosDB aan de hand van query en het vergelijken van de gegevens---//
                var result = client.CreateDocumentQuery<JObject>(collectionUrl, query, queryOptions).AsEnumerable();
                if (result.Count() == 0)
                {
                    return new OkObjectResult(false);
                }
                else
                {
                    return new OkObjectResult(true);
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
