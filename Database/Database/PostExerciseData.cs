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
using System.Diagnostics;

namespace Database
{
    //---------------------------------------------------------------------------------------//
    //------------------------------Aanmaken van Exercise data-------------------------------//
    //---------------------------------------------------------------------------------------//

    public static class PostExerciseData
    {
        [FunctionName("PostExerciseData")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                //---Ophalen van body en deserializeren---//
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                Exercise exe = JsonConvert.DeserializeObject<Exercise>(requestBody);

                //---Connectie met CosmosDB voorbereiden---//
                Uri serviceEndPoint = new Uri(Environment.GetEnvironmentVariable("CosmosEndPoint"));
                string key = Environment.GetEnvironmentVariable("CosmosKey");

                //---Connectie met CosmosDB---//
                DocumentClient client = new DocumentClient(serviceEndPoint, key);
                var collectionUrl = UriFactory.CreateDocumentCollectionUri("streetworkout", "Data");

                //---Doorsturen van data naar CosmosDB---//
                await client.CreateDocumentAsync(collectionUrl, exe);
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
