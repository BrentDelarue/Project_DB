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
    //---------------------------------------------------------------------------------------//
    //----------------------------Verwijderen van ProfilePicture-----------------------------//
    //---------------------------------------------------------------------------------------//

    public static class DeleteProfilePicture
    {
        [FunctionName("DeleteProfilePicture")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "DeleteProfilePicture/{value}")] HttpRequest req, string value,
            ILogger log)
        {
            try
            {
                //---Connectie met BlobStorage voorbereiden---//
                CloudStorageAccount _cloudStorageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("BlobStorageAccount"));
                CloudBlobClient blobClient = _cloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference("profilepicture");


                //---Voorbereiden en deleten van ProfilePicture---//
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(value + ".jpg");
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
