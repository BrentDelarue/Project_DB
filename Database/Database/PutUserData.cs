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
    //---------------------------------------------------------------------------------------//
    //---------------------------------Updaten van User data---------------------------------//
    //---------------------------------------------------------------------------------------//

    public static class PutUserData
    {
        [FunctionName("PutUserData")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                //---Ophalen van body en deserializeren---//
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                JObject userData = JsonConvert.DeserializeObject<JObject>(requestBody);

                //---Connectie met CosmosDB voorbereiden en maken---//
                Uri serviceEndPoint = new Uri(Environment.GetEnvironmentVariable("CosmosEndPoint"));
                string key = Environment.GetEnvironmentVariable("CosmosKey");
                DocumentClient client = new DocumentClient(serviceEndPoint, key);
                var collectionUrl = UriFactory.CreateDocumentCollectionUri("streetworkout", "Data");

                //---Querie voorbereiden---//
                FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true };
                string query = $"SELECT * FROM c WHERE c.{userData["Reference"]} = \"{userData[userData["Reference"].ToString()]}\" and c.Type = \"User\"";

                //---Ophalen van data uit CosmosDB aan de hand van query en het vergelijken van de gegevens---//
                JObject result = client.CreateDocumentQuery<JObject>(collectionUrl, query, queryOptions).AsEnumerable().FirstOrDefault();
                if (userData["Name"] != null)
                {
                    result["Name"] = userData["Name"];
                }
                if (userData["ApiName"] != null)
                {
                    result["ApiName"] = userData["ApiName"];
                }
                if (userData["Password"] != null)
                {
                    result["Password"] = userData["Password"];
                }
                if (userData["Weight"] != null)
                {
                    result["Weight"] = userData["Weight"];
                }
                if (userData["Length"] != null)
                {
                    result["Length"] = userData["Length"];
                }
                if (userData["Age"] != null)
                {
                    result["Age"] = userData["Age"];
                }
                if (userData["Image"] != null)
                {
                    result["Image"] = userData["Image"];
                }

                //---Document in CosmosDB updaten met nieuwe waarden---//
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
