using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using EventManagerDemo.Models;
using EventManagerDemo.Utils;
using Microsoft.Graph;

namespace EventManagerDemo.Controllers
{
    public class ImagesController : ApiController
    {        
        GraphServiceClient graphClient;

        public ImagesController()
        {
            graphClient = SDKHelper.GetAuthenticatedClient();            
        }

        [Authorize]
        [Route("images/GetGroupImage")]
        public async Task<HttpResponseMessage> GetGroupImage(string groupid)
        {            
            GroupsRepository groupsRepository = new GroupsRepository(graphClient);

            try
            {
                byte[] imgData = await groupsRepository.GetGroupPhoto(groupid);
                MemoryStream ms = new MemoryStream(imgData);
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StreamContent(ms);
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
                return response;
            }
            catch (Exception e)
            {
                throw e;
            }
            
        }

        [Authorize]
        [Route("images/GetVideoThumbnail")]
        public async Task<HttpResponseMessage> GetVideoThumbnail(string channelid, string videoid, string preferredwidth)
        {
            var accessTokenSharePoint = await AadHelper.GetAccessTokenForSharePoint();
            var repoSP = new VideoChannelRepository(accessTokenSharePoint);
            byte[] imgData = await repoSP.GetVideoThumbnail(channelid, videoid, preferredwidth);
            if (imgData == null) return null;
            MemoryStream ms = new MemoryStream(imgData);
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(ms);
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
            return response;
        }
    }
}
