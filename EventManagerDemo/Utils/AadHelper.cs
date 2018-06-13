using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using EventManagerDemo.Models;
using System;

namespace EventManagerDemo.Utils
{
    public class AadHelper
    {        
        public static async Task<string> GetAccessTokenForSharePoint()
        {            
            // fetch from stuff user claims
            var userObjectId = ClaimsPrincipal.Current.FindFirst(SettingsHelper.ClaimTypeObjectIdentifier).Value;

            // discover contact endpoint
            var clientCredential = new ClientCredential(SettingsHelper.ClientId, SettingsHelper.ClientSecret);
            var userIdentifier = new UserIdentifier(userObjectId, UserIdentifierType.UniqueId);

            // create auth context
            AuthenticationContext authContext = new AuthenticationContext(SettingsHelper.AzureADAuthority,
                new EfAdalTokenCache(userObjectId));

            // authenticate
            var authResult =
                await
                authContext.AcquireTokenSilentAsync(
                    string.Format("https://{0}.sharepoint.com", SettingsHelper.Office365TenantId), clientCredential,
                    userIdentifier);
            
            return authResult.AccessToken;
        }
        public static async Task<string> GetAccessTokenForMicrosoftGraph()
        {
            // fetch from stuff user claims
            var userObjectId = ClaimsPrincipal.Current.FindFirst(SettingsHelper.ClaimTypeObjectIdentifier).Value;
            return await GetAccessTokenFromGraphRefreshTokenAsync(userObjectId).ConfigureAwait(continueOnCapturedContext: false);
        }

        public static async Task<string> GetAccessTokenFromGraphRefreshTokenAsync(string userObjectId)
        {
            string authority = SettingsHelper.AzureADAuthority;
            AuthenticationContext authContext = new AuthenticationContext(authority, new EfAdalTokenCache(userObjectId));
            var clientCredential = new ClientCredential(SettingsHelper.ClientId, SettingsHelper.ClientSecret);
            var userIdentifier = new UserIdentifier(userObjectId, UserIdentifierType.UniqueId);
            AuthenticationResult authResult = await authContext.AcquireTokenSilentAsync(
                SettingsHelper.GraphUri, clientCredential, userIdentifier).ConfigureAwait(continueOnCapturedContext: false);
            return authResult.AccessToken;
        }

    }
}