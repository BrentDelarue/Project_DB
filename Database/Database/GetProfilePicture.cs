using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace Database
{
    //---------------------------------------------------------------------------------------//
    //------------------------------Ophalen van ProfilePicture-------------------------------//
    //---------------------------------------------------------------------------------------//

    public static class GetProfilePicture
    {
        [FunctionName("GetProfilePicture")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetProfilePicture/{value}")] HttpRequest req, string value,
            ILogger log)
        {
            try
            {
                //---Connectie met BlobStorage voorbereiden---//
                CloudStorageAccount _cloudStorageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("BlobStorageAccount"));
                CloudBlobClient blobClient = _cloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference("profilepicture");

                //---Voorbereiden ophalen van ProfilePicture---//
                SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy();
                sasConstraints.SharedAccessStartTime = DateTimeOffset.UtcNow.AddMinutes(-5);
                sasConstraints.SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddHours(24);
                sasConstraints.Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write;
                string sasBlobToken = container.GetSharedAccessSignature(sasConstraints);

                //---Ophalen van gegevens uit Blobstorage en terug geven---//
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(value + ".jpg");
                JObject url = new JObject();
                url["Uri"] = blockBlob.Uri;
                url["SAS"] = sasBlobToken;
                return new OkObjectResult(url);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
                throw ex;
            }
        }
    }
}
