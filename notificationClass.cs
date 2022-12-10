using AdaptiveCards.Templating;
using Newtonsoft.Json;
using powerFlexBackup.cipdevice;
using System.Text;

namespace powerFlexBackup.notifications
{
    public class notification
    {

        public notification(){}
        private static readonly HttpClient client = new HttpClient();
        public static void sendNotificationOnNewDeviceIdentity(IdentityObject identityObject, string host){
            
            identityObject.Host = host;
            Console.WriteLine(JsonConvert.SerializeObject(identityObject, Formatting.Indented));

            var templateJson = @"                    
                {
   ""type"":""message"",
   ""attachments"":[
      {
         ""contentType"":""application/vnd.microsoft.card.adaptive"",
         ""contentUrl"":null,
         ""content"":{
            ""type"":""AdaptiveCard"",
            ""version"":""1.5"",
            ""body"":[
               {
                  ""type"":""TextBlock"",
                  ""text"":""New device type discovered"",
                  ""weight"":""bolder"",
                  ""size"":""medium""
               },
               {
                  ""type"":""TextBlock"",
                  ""text"":""Host: ${Host}""
               },
               {
                  ""type"":""TextBlock"",
                  ""text"":""VendorID: ${VendorID}""
               },
               {
                  ""type"":""TextBlock"",
                  ""text"":""DeviceType ${DeviceType}""
               },
               {
                  ""type"":""TextBlock"",
                  ""text"":""ProductCode ${ProductCode}""
               },
               {
                  ""type"":""TextBlock"",
                  ""text"":""revision ${Revision.Major}.${Revision.Minor}""
               },
               {
                  ""type"":""TextBlock"",
                  ""text"":""SerialNumber ${SerialNumber}""
               },
               {
                  ""type"":""TextBlock"",
                  ""text"":""ProductName ${ProductName}""
               }
            ]
         }
      }
   ]
}";

            // Create a Template instance from the template payload
            AdaptiveCardTemplate template = new AdaptiveCardTemplate(templateJson);

            // "Expand" the template - this generates the final Adaptive Card payload
            string cardJson = template.Expand(identityObject);

            var content = new StringContent(cardJson.ToString(), Encoding.UTF8, "application/json");
            var url = "https://hormel.webhook.office.com/webhookb2/81e561c9-5577-4c2c-aa5c-48318e2f81c9@9ecd565a-141c-4d9e-820d-886b40282cf2/IncomingWebhook/f7f064ab03b74ccaa97daa5510d71979/09582dd3-ecb4-4fe5-806e-5d027d0b9aa9";
            var webRequest = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content,

            };

            var response = client.Send(webRequest);
            Thread.Sleep(2500);
        }

    }
}
