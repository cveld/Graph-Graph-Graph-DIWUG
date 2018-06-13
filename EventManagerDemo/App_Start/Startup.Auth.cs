using System;
using System.Threading.Tasks;
using System.Web;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using EventManagerDemo.Utils;
using System.Data;

namespace EventManagerDemo
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            // configure the authentication type & settings
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            // configure the OWIN OpenId Connect options
            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                ClientId = SettingsHelper.ClientId,
                Authority = SettingsHelper.AzureADAuthority,
                RedirectUri = SettingsHelper.RedirectUri,                
                Notifications = new OpenIdConnectAuthenticationNotifications()
                {
                    // when an auth code is received...
                    AuthorizationCodeReceived = async (context) =>
                    {
                        // get the OpenID Connect code passed from Azure AD on successful auth
                        string code = context.Code;


                        // create the app credentials & get reference to the user
                        ClientCredential creds = new ClientCredential(SettingsHelper.ClientId, SettingsHelper.ClientSecret);
                        string userObjectId = context.AuthenticationTicket.Identity.FindFirst(SettingsHelper.ClaimTypeObjectIdentifier).Value;

                        // use the ADAL to obtain access token & refresh token...
                        //  save those in a persistent store...
                        EfAdalTokenCache tokenCache = new EfAdalTokenCache(userObjectId);
                        AuthenticationContext authContext = new AuthenticationContext(SettingsHelper.AzureADAuthority, tokenCache);

                        // obtain access token for the AzureAD graph
                        Uri redirectUri = new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path));
                        AuthenticationResult authResult = await authContext.AcquireTokenByAuthorizationCodeAsync(code, redirectUri, creds, SettingsHelper.AzureAdGraphResourceId);

                        // successful auth
                        //return authResult;//Task.FromResult(0);
                    },
                    AuthenticationFailed = (context) =>
                    {
                        context.HandleResponse();

                        string debug = context.Exception != null ? context.Exception.Message : String.Empty;
                        if (context.Exception.InnerException != null)
                        {
                            debug += System.Environment.NewLine + context.Exception.InnerException.Message;
                        }

                        if (context.Exception is DataException)
                        {
                            context.Response.Redirect($"/Error?Message=Database failed&Debug={debug}");
                        } else
                        {
                            context.Response.Redirect($"/Error?Message=Authentication failed&Debug={debug}");
                        }

                        return Task.FromResult(0);
                    }
                },
                TokenValidationParameters = new System.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = false
                }
            });
        }

    }
}