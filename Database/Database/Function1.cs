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
using System.Text;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Reflection.Metadata;

namespace Database
{
    public static class UpdateProfile
    {

        [FunctionName("UpdateProfile")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req, ILogger log)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                Gebruiker user = JsonConvert.DeserializeObject<Gebruiker>(requestBody);
                Uri serviceEndPoint = new Uri(Environment.GetEnvironmentVariable("CosmosEndPoint"));
                string key = Environment.GetEnvironmentVariable("CosmosKey");
                DocumentClient client = new DocumentClient(serviceEndPoint, key);
                var collectionUrl = UriFactory.CreateDocumentCollectionUri("streetworkout", "Users");
                FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true };
                string query = $"SELECT * FROM c WHERE c.Naam = \"{user.Naam}\" and c.Wachtwoord = \"{user.Wachtwoord}\"";
                var result = client.CreateDocumentQuery<JObject>(collectionUrl, query, queryOptions).AsEnumerable().FirstOrDefault();
                if (user.Gewicht != null)
                {
                    result["Gewicht"] = user.Gewicht;
                }
                if (user.Lengte != null)
                {
                    result["Lengte"] = user.Lengte;
                }
                if (user.Leeftijd != null)
                {
                    result["Leeftijd"] = user.Leeftijd;
                }
                //if (user.Achievements.Count() != 0)
                //{
                //    foreach (string achievement in user.Achievements)
                //    {
                //        result["Gewicht"].Add(achievement);
                //    }
                //}
                Debug.WriteLine(result["user"]);
                var response = await client.DeleteDocumentAsync(UriFactory.CreateDocumentUri("streetworkout", "Users", result["user"].ToString()));
                await client.CreateDocumentAsync(collectionUrl, result);
                return new OkObjectResult("cv");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
                throw ex;
            }
        }
    }
}
