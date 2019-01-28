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
    //---------------------------------Ophalen van User data---------------------------------//
    //---------------------------------------------------------------------------------------//

    public static class GetUserData
    {
        [FunctionName("GetUserData")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetUserData/{value}/{reference}")] HttpRequest req, string value, string reference,
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
                string query = $"SELECT * FROM c WHERE c.{reference} = \"{value}\" and c.Type = \"User\"";

                //---Ophalen van data uit CosmosDB en terug geven---//
                UserSave result = client.CreateDocumentQuery<UserSave>(collectionUrl, query, queryOptions).AsEnumerable().FirstOrDefault();
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
