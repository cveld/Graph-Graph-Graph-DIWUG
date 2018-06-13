using EventManagerDemo.Models;
using EventManagerDemo.Utils;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace EventManagerDemo.Services
{
    // Life-time: contruct in the Controller action method; only then is the Controller Session object available
    public class SubscriptionsRepository
    {
        GraphServiceClient graphClient;
        Cache cache;
        HttpSessionStateBase sessionState;


        // TO DO; inject additional dependencies ConfigurationManager.AppSettings/notificationurl and ClaimsPrincipal.Current/userObjectId
        public SubscriptionsRepository(GraphServiceClient graphClient, HttpSessionStateBase session, Cache cache)
        {
            this.graphClient = graphClient;
            this.sessionState = session;
            this.cache = cache;
        }

        const string SubscriptionIdKey = "SubScriptionId";
        // Create a webhook subscription. This action method is called from         
        public async Task<Microsoft.Graph.Subscription> CreateOrReuseSubscription()
        {
            var subscriptionId = sessionState[SubscriptionIdKey] as string;
            if (subscriptionId != null)
            {
                return new Microsoft.Graph.Subscription
                {
                    Id = subscriptionId
                };
            }

            var subscription = new Microsoft.Graph.Subscription
            {
                Resource = "me/mailFolders('Inbox')/messages",
                ChangeType = "created",
                NotificationUrl = ConfigurationManager.AppSettings["ida:NotificationUrl"],
                ClientState = Guid.NewGuid().ToString(),
                ExpirationDateTime = DateTime.UtcNow + new TimeSpan(0, 0, 4230, 0)
            };

            var result = await graphClient.Subscriptions.Request().AddAsync(subscription);

            // This app temporarily stores the current subscription ID and client state. 
            // These are required so the NotificationController, which is not authenticated, can retrieve an access token keyed from the subscription ID.
            // Production apps typically use some method of persistent storage.

            SubscriptionData data = new Models.SubscriptionData
            {
                ClientState = result.ClientState,
                userObjectId = ClaimsPrincipal.Current.FindFirst(SettingsHelper.ClaimTypeObjectIdentifier).Value
            };

            cache.Insert("subscriptionId_" + result.Id,
                data, null, DateTime.MaxValue, new TimeSpan(24, 0, 0), System.Web.Caching.CacheItemPriority.NotRemovable, null);

            // Save the latest subscription ID, so we can delete it later.
            sessionState[SubscriptionIdKey] = result.Id;
            return result;


        }

        public async Task DeleteSubscription()
        {
            var subscriptionId = sessionState[SubscriptionIdKey] as string;

            if (!string.IsNullOrEmpty(subscriptionId))
            {
                await graphClient.Subscriptions[subscriptionId].Request().DeleteAsync().ConfigureAwait(continueOnCapturedContext: false);
            }
        }
    }
}