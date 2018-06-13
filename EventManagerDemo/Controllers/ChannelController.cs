using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EventManagerDemo.Models;
using EventManagerDemo.Utils;

namespace EventManagerDemo.Controllers {
  public class ChannelController : Controller {
    [Authorize]
    public async Task<ActionResult> Index() {
      var accessToken = await AadHelper.GetAccessTokenForSharePoint();
      var repo = new VideoChannelRepository(accessToken);

      var channels = await repo.GetChannels(false);

      return View(channels);
    }
  }
}