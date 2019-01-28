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
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Database
{
    //---------------------------------------------------------------------------------------//
    //------------------------Opstellen van mail en nieuw wachtwoord-------------------------//
    //---------------------------------------------------------------------------------------//

    public static class MailService
    {
        [FunctionName("MailService")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                //---Ophalen van body en deserializeren---//
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                JObject user = JsonConvert.DeserializeObject<JObject>(requestBody);

                //---Opstellen van een nieuw wachtwoord---//
                Random rnd = new Random();
                int length = rnd.Next(10, 15);
                string password = "";
                for (int i = 0; i < length; i++)
                {
                    Random rnd2 = new Random();
                    int number = rnd.Next(3, 6);
                    if (number % 3 == 0)
                    {
                        Random rnd3 = new Random();
                        string element = rnd.Next(0, 10).ToString();
                        password += element;
                    }
                    else if (number % 4 == 0)
                    {
                        var chars = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
                        Random rnd3 = new Random();
                        int index = rnd.Next(0, chars.Length);
                        password += chars[index];
                    }
                    else if (number % 5 == 0)
                    {
                        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
                        Random rnd3 = new Random();
                        int index = rnd.Next(0, chars.Length);
                        password += chars[index];
                    }
                }

                //---Opstellen van de mail naar de user---//
                var client = new SendGridClient(Environment.GetEnvironmentVariable("SendGrid_API"));
                var from = new EmailAddress("nmctstreetworkoutreset@outlook.com", "StreetWorkout");
                var subject = "Aanvraag voorlopig wachtwoord";
                var to = new EmailAddress(user["Email"].ToString(), user["Name"].ToString());
                var plainTextContent = $"Beste {user["Name"].ToString()}{Environment.NewLine}{Environment.NewLine}\nU heeft een nieuw wachtwoord aangevraagd in de app StreetWorkout.\nVolgend wachtwoord is uw nieuw voorlopig wachtwoord: {password}\r\nWe raden u tensterkste aan om uw wachtwoord te veranderen na het gebruiken van dit wachtwoord.\n\nGroeten support StreetWorkout.";
                var htmlContent = $"Beste {user["Name"].ToString()}<br><br>U heeft een nieuw wachtwoord aangevraagd in de app StreetWorkout.<br>Uw nieuw voorlopig wachtwoord: {password}<br>We raden u tensterkste aan om uw wachtwoord te veranderen na het gebruiken van dit wachtwoord.<br><br>Groeten support StreetWorkout.";
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

                //---Versturen van de mail en nieuw wachtwoord terug geven---//
                var response = await client.SendEmailAsync(msg);
                return new OkObjectResult(password);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
                throw ex;
            }
        }
    }
}
