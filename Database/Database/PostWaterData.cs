using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace Database
{
    //---------------------------------------------------------------------------------------//
    //-------------------------------Aanmaken van Water data---------------------------------//
    //---------------------------------------------------------------------------------------//

    public static class PostWaterData
    {
        [FunctionName("PostWaterData")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                //---Ophalen van body en deserializeren---//
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                Water water = JsonConvert.DeserializeObject<Water>(requestBody);

                //---Connectie met CosmosDB voorbereiden---//
                Uri serviceEndPoint = new Uri(Environment.GetEnvironmentVariable("CosmosEndPoint"));
                string key = Environment.GetEnvironmentVariable("CosmosKey");

                //---Connectie met CosmosDB---//
                DocumentClient client = new DocumentClient(serviceEndPoint, key);
                var collectionUrl = UriFactory.CreateDocumentCollectionUri("streetworkout", "Data");

                //---Doorsturen van data naar CosmosDB---//
                await client.CreateDocumentAsync(collectionUrl, water);
                return new StatusCodeResult(200);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
                throw ex;
            }
        }
    }
}
