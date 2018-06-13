using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using EventManagerDemo.Models;
using EventManagerDemo.SignalR;
using EventManagerDemo.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Graph;
using System.Threading;
using System.Web.Caching;

namespace EventManagerDemo.Controllers
/*
 *  Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
 *  See LICENSE in the source repository root for complete license information.
 */
{
    public class NotificationController : Controller
    {
        public ActionResult LoadView()
        {
            return View("Notification");
        }

        // The notificationUrl endpoint that's registered with the webhook subscription.
        [HttpPost]
        public async Task<ActionResult> Listen()
        {

            // Validate the new subscription by sending the token back to Microsoft Graph.
            // This response is required for each subscription.
            if (Request.QueryString["validationToken"] != null)
            {
                var token = Request.QueryString["validationToken"];
                return Content(token, "plain/text");
            }

            // Parse the received notifications.
            else
            {
                try
                {
                    var notifications = new Dictionary<string, Notification>();
                    using (var inputStream = new System.IO.StreamReader(Request.InputStream))
                    {
                        JObject jsonObject = JObject.Parse(inputStream.ReadToEnd());
                        if (jsonObject != null)
                        {

                            // Notifications are sent in a 'value' array.
                            JArray value = JArray.Parse(jsonObject["value"].ToString());
                            foreach (var notification in value)
                            {
                                Notification current = JsonConvert.DeserializeObject<Notification>(notification.ToString());

                                // Check client state to verify the message is from Microsoft Graph. 
                                var subscriptionData = (SubscriptionData)HttpRuntime.Cache.Get("subscriptionId_" + current.SubscriptionId);
                                if (subscriptionData != null)
                                {
                                    if (current.ClientState == subscriptionData.ClientState)
                                    {
                                        // Just keep the latest notification for each resource.
                                        // No point pulling data more than once.
                                        notifications[current.Resource] = current;
                                    }
                                }
                            }

                            if (notifications.Count > 0)
                            {
                                // Query for the changed messages. 
                                await GetChangedMessagesAsync(notifications.Values, HttpRuntime.Cache);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                    // TODO: Handle the exception.
                    // Still return a 202 so the service doesn't resend the notification.
                }
                return new HttpStatusCodeResult(202);
            }
        }

        // Get information about the changed messages and send to browser via SignalR.
        // A production application would typically queue a background job for reliability.
        [HandleAdalException]
        public async Task GetChangedMessagesAsync(IEnumerable<Notification> notifications, Cache cache)
        {
            List<Microsoft.Graph.Message> messages = new List<Microsoft.Graph.Message>();            

            foreach (var notification in notifications)
            {
                // Get the userObjectId from the cached SubscriptionData and create a Graph SDK authenticated client
                var subscriptionData = (SubscriptionData)cache.Get("subscriptionId_" + notification.SubscriptionId);
                var graphClient = SDKHelper.GetAuthenticatedClient(subscriptionData.userObjectId);

                var request = new BaseRequest($"{graphClient.BaseUrl}/{notification.Resource}", graphClient);
                request.ContentType = "application/json";

                var response = await request.SendRequestAsync(null, CancellationToken.None).ConfigureAwait(continueOnCapturedContext: false);

                // Get the messages from the JSON response.                
                string stringResult = await response.Content.ReadAsStringAsync();
                var type = notification.ResourceData.ODataType;
                if (type == "#Microsoft.Graph.Message") {
                        messages.Add(JsonConvert.DeserializeObject<Microsoft.Graph.Message>(stringResult));
                }
                
            }

            if (messages.Count > 0)
            {
                NotificationService notificationService = new NotificationService();
                notificationService.SendNotificationToClient(messages);
            }
        }
    }
}
