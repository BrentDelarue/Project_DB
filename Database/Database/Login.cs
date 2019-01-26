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
    public static class Login
    {
        [FunctionName("Login")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Login/{naam}/{wachtwoord}")] HttpRequest req, string naam, string wachtwoord,
            ILogger log)
        {
            try
            {
                Uri serviceEndPoint = new Uri(Environment.GetEnvironmentVariable("CosmosEndPoint"));
                string key = Environment.GetEnvironmentVariable("CosmosKey");
                DocumentClient client = new DocumentClient(serviceEndPoint, key);
                var collectionUrl = UriFactory.CreateDocumentCollectionUri("streetworkout", "Data");
                FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true };
                string query = $"SELECT * FROM c WHERE c.Naam = \"{naam}\" and c.Wachtwoord = \"{wachtwoord}\" and c.Type = \"Gebruiker\"";
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
