/*
 *  Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
 *  See LICENSE in the source repository root for complete license information.
 */

using System;
using System.Web;
using System.Web.Mvc;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using EventManagerDemo.Utils;
using EventManagerDemo.Models;
using System.Security.Claims;
using Microsoft.Graph;
using System.Web.Caching;
using EventManagerDemo.Services;
using System.Web.Routing;

namespace EventManagerDemo.Controllers
{
    public class SubscriptionController : Controller
    {
        GraphServiceClient graphClient;
        SubscriptionsRepository subscriptionsRepository;

        public SubscriptionController()
        {
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            graphClient = SDKHelper.GetAuthenticatedClient();
            subscriptionsRepository = new SubscriptionsRepository(graphClient, this.Session, HttpRuntime.Cache);
        }

        public ActionResult Index()
        {            
            return View();
        }

        [Authorize, HandleAdalException]
        public async Task<ActionResult> CreateSubscription()
        {
            try
            {
                var subscription = await subscriptionsRepository.CreateOrReuseSubscription();
                var viewModel = new SubscriptionViewModel();
                viewModel.Subscription = subscription;
                return View("Subscription", viewModel);
            }
            catch (Exception ex)
            {
                if (ex is AdalException) throw ex;
                return RedirectToAction("Index", "Error", new { message = ex.Message });
            }
        }

        

        // Delete the current webhooks subscription and sign out the user.
        [Authorize, HandleAdalException]
        public async Task<ActionResult> DeleteSubscription()
        {
            string subscriptionId = (string)Session["SubscriptionId"];
            if (!string.IsNullOrEmpty(subscriptionId))
            {
                await subscriptionsRepository.DeleteSubscription();
            }

            return RedirectToAction("SignOut", "Account");
        }
    }
}