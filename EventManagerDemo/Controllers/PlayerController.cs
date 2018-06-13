using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EventManagerDemo.Models;
using EventManagerDemo.Utils;

namespace EventManagerDemo.Controllers
{
    public class PlayerController : Controller
    {
        // GET: Player
        [Authorize, HandleAdalException]
        public async Task<ActionResult> Index(string channelId, string videoId)
        {
            var accessToken = await AadHelper.GetAccessTokenForSharePoint();
            var repo = new VideoChannelRepository(accessToken);
            var video = await repo.GetVideo(channelId, videoId);
            var playerModel = new PlayerViewModel
            {
                ChannelId = video.ChannelId,
                VideoId = video.VideoId,
                Title = video.Title,
                DisplayFormUrl = video.DisplayFormUrl,
                ThumbnailUrl = video.ThumbnailUrl
            };
            return View(playerModel);
        }
    }
}