using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EventManagerDemo.Models;
using EventManagerDemo.Utils;
using Microsoft.Graph;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace EventManagerDemo.Controllers
{
    public class GroupsController : Controller
    {
        GraphServiceClient graphClient;

        public GroupsController()
        {
            graphClient = SDKHelper.GetAuthenticatedClient();
        }

        // GET: Groups
        [Authorize, HandleAdalException]
        public async Task<ActionResult> Index()
        {            
            var groupsRepository = new GroupsRepository(graphClient);
            try
            {
                var groups = await groupsRepository.GetGroups();
                return View(groups);
            }
            catch (Exception ex)
            {
                if (ex is AdalException)
                {
                    throw ex;
                }
                return RedirectToAction("Index", "Error", new { message = ex.Message });
            }
        }
    }
}