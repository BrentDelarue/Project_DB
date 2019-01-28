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
    //---------------------------------------------------------------------------------------//
    //--------------------------Aanmaken/Updaten van ProfielPicture--------------------------//
    //---------------------------------------------------------------------------------------//

    public static class PostProfilePicture
    {
        [FunctionName("PostProfilePicture")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                //---Ophalen van body en deserializeren en stream maken van image---//
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                ProfilePicture image = JsonConvert.DeserializeObject<ProfilePicture>(requestBody);
                MemoryStream stream = new MemoryStream(image.stream);

                //---Connectie met BlobStorage voorbereiden---//
                CloudStorageAccount _cloudStorageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("BlobStorageAccount"));
                CloudBlobClient blobClient = _cloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference("profilepicture");

                //---Doorsturen van data naar BlobStorage---//
                CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(image.Name); cloudBlockBlob.Properties.ContentType = "image/jpg";
                await cloudBlockBlob.UploadFromStreamAsync(stream);
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
