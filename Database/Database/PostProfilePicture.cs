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
using System.Diagnostics;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Text;

namespace Database
{
    public static class PostProfilePicture
    {
        [FunctionName("PostProfilePicture")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                ProfilePicture image = JsonConvert.DeserializeObject<ProfilePicture>(requestBody);
                CloudStorageAccount _cloudStorageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("BlobStorageAccount"));
                CloudBlobClient blobClient = _cloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference("profilepicture");
                CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(image.Naam); cloudBlockBlob.Properties.ContentType = "image/jpg";
                await cloudBlockBlob.UploadFromStreamAsync(image.stream);
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
