using Microsoft.Graph;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace EventManagerDemo.Models
{
    public class GraphService
    {
        GraphServiceClient graphClient;

        public GraphService(GraphServiceClient graphClient)
        {
            this.graphClient = graphClient;
        }

        public async Task<string> GetMyEmailAddress(GraphServiceClient graphClient)
        {

            // Get the current user. 
            // This sample only needs the user's email address, so select the mail and userPrincipalName properties.
            // If the mail property isn't defined, userPrincipalName should map to the email for all account types. 
            User me = await graphClient.Me.Request().Select("mail,userPrincipalName").GetAsync();
            return me.Mail ?? me.UserPrincipalName;
        }


        /// <summary>
        /// Example how to set values within an Open Extension. TO DO: Make it working
        /// </summary>
        /// <param name="graphClient"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task PatchExtension(GraphServiceClient graphClient, string id)
        {
            // PATCH https://graph.microsoft.com/v1.0/groups/{id}/extensions('Com.Contoso.Referral')

            var groupExtensionRequest = new JsonHelpers.GroupExtensionRequest
            {
                myvalue = "testvalue"
            };

            //var serializer = new JsonSerializer();
            var jsonRequest = JsonConvert.SerializeObject(groupExtensionRequest);



            //using (var requestMessage = new HttpRequestMessage(patchmethod, $"https://graph.microsoft.com/v1.0/groups/{id}/extensions('{groupExtensionRequest.extensionName}')"))
            {
                //if (response.StatusCode == HttpStatusCode.OK) 
                var request = new BaseRequest($"https://graph.microsoft.com/v1.0/groups/{id}/extensions('{groupExtensionRequest.extensionName}')", graphClient);
                request.Method = "PATCH";
                request.ContentType = "application/json";
                var response = await request.SendRequestAsync(jsonRequest, CancellationToken.None);

                var resultString = response.Content.ReadAsStringAsync().Result;
                var jsonResponse = JsonConvert.DeserializeObject<JsonHelpers.GroupExtensionResponse>(resultString);
            }
        }
    }
}