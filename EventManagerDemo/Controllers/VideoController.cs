/* Test */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using EventManagerDemo.Models;
using EventManagerDemo.Utils;
using Microsoft.Graph;
using EventManagerDemo.Services;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace EventManagerDemo.Controllers
{
    public class VideoController : Controller
    {
        GraphServiceClient graphClient;      

        public VideoController()
        {
            graphClient = SDKHelper.GetAuthenticatedClient();            
        }

        [Authorize, HandleAdalException]
        public async Task<ActionResult> Index(string channelId)
        {
            try
            {
                var accessTokenSharePoint = await AadHelper.GetAccessTokenForSharePoint();
                var accessTokenMSGraph = await AadHelper.GetAccessTokenForMicrosoftGraph();

                var sharePointRepository = new VideoChannelRepository(accessTokenSharePoint);
                var groupsRepository = new GroupsRepository(graphClient);
                var subscriptionsRepository = new SubscriptionsRepository(graphClient, this.Session, HttpRuntime.Cache);

                var channel = await sharePointRepository.GetChannel(channelId);
                var videos = await sharePointRepository.GetChannelVideos(channelId);
                var groups = await groupsRepository.GetGroups();

                var viewModel = new VideoListViewModel
                {
                    ChannelId = channelId,
                    ChannelTitle = channel.Title,
                    Videos = videos,
                    Groups = groups
                };

                // integration of the webhooks example                
                Microsoft.Graph.Subscription result = await subscriptionsRepository.CreateOrReuseSubscription();

                return View(viewModel);
            }
            catch (Exception ex)
            {
                if (ex is AdalException)
                {
                    // let the ActionFilterAttribute take care of it
                    throw ex;   
                }

                return RedirectToAction("Index", "Error", new { message = ex.Message });
            }
        }

        [Authorize, HandleAdalException]
        public async Task<ActionResult> Create(string channelId)
        {
            var video = new Models.Video
            {
                ChannelId = channelId
            };

            return View(video);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ChannelId,Title,Description")] Models.Video video, HttpPostedFileBase upload)
        {
            var accessToken = await AadHelper.GetAccessTokenForSharePoint();
            var repo = new VideoChannelRepository(accessToken);

            // if a file is uploaded, add to video & upload
            if (upload != null && upload.ContentLength > 0)
            {
                video.FileName = upload.FileName;
                using (var reader = new System.IO.BinaryReader(upload.InputStream))
                {
                    video.FileContent = reader.ReadBytes(upload.ContentLength);
                }

                await repo.UploadVideo(video);
            }

            return RedirectToRoute("ChannelVideos", new RouteValueDictionary(new { channelId = video.ChannelId, action = "Index" }));
        }

        [Authorize]
        public async Task<ActionResult> Delete(string channelId, string videoId)
        {
            var accessToken = await AadHelper.GetAccessTokenForSharePoint();
            var repo = new VideoChannelRepository(accessToken);

            if (channelId != null && videoId != null)
            {
                await repo.DeleteChannelVideo(channelId, videoId);
            }

            // if channelid provided, use this
            if (channelId != null)
            {
                return RedirectToRoute("ChannelVideos", new RouteValueDictionary(new { channelId = channelId, action = "Index" }));
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Channel", action = "Index" });
            }
        }
    }
}