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
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Diagnostics;

namespace Database
{
    public static class DeleteProfilePicture
    {
        [FunctionName("DeleteProfilePicture")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                JObject naam = JsonConvert.DeserializeObject<JObject>(requestBody);
                CloudStorageAccount _cloudStorageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("BlobStorageAccount"));

                CloudBlobClient blobClient = _cloudStorageAccount.CreateCloudBlobClient();

                CloudBlobContainer container = blobClient.GetContainerReference("profilepicture");
                
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(naam["Naam"] + ".jpg");
                await blockBlob.DeleteIfExistsAsync();
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
