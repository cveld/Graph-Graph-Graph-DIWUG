using System.Net.Http.Headers;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EventManagerDemo.Models;

namespace EventManagerDemo.Utils
{
    public class SDKHelper
    {
        //private static GraphServiceClient graphClient = null;

        // Get an authenticated Microsoft Graph Service client and takes userObjectid from claims
        public static GraphServiceClient GetAuthenticatedClient()
        {
            GraphServiceClient graphClient = new GraphServiceClient(//"https://graph.microsoft.com/beta",
                new DelegateAuthenticationProvider(
                    async (requestMessage) =>
                    {
                        string accessToken = //await SampleAuthProvider.Instance.GetUserAccessTokenAsync();
                        await AadHelper.GetAccessTokenForMicrosoftGraph();

                        // Append the access token to the request.
                        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", accessToken);

                    // This header has been added to identify our sample in the Microsoft Graph service. If extracting this code for your project please remove.
                    //requestMessage.Headers.Add("SampleID", "aspnet-connect-sample");
                    }));
            return graphClient;
        }

        public static void SignOutClient()
        {
            //graphClient = null;
        }

        public static GraphServiceClient GetAuthenticatedClient(string userObjectId)
        {
            GraphServiceClient graphClient = new GraphServiceClient(//"https://graph.microsoft.com/beta",
                new DelegateAuthenticationProvider(
                    async (requestMessage) =>
                    {
                        string accessToken = //await SampleAuthProvider.Instance.GetUserAccessTokenAsync();
                        await AadHelper.GetAccessTokenFromGraphRefreshTokenAsync(userObjectId);

                        // Append the access token to the request.
                        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", accessToken);

                        // This header has been added to identify our sample in the Microsoft Graph service. If extracting this code for your project please remove.
                        //requestMessage.Headers.Add("SampleID", "aspnet-connect-sample");
                    }));
            return graphClient;
        }
    }
}
