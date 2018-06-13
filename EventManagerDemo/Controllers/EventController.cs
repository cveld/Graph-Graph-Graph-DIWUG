using EventManagerDemo.Models;
using EventManagerDemo.Utils;

using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EventManagerDemo.Controllers
{
    public class EventController : Controller
    {
        GraphServiceClient graphClient; 
        ManagedEventRepository managedEventRepository;
        SchemaExtensionsRepository schemaExtensionsRepository;
        GraphService graphService;

        public EventController()
        {
            graphClient = SDKHelper.GetAuthenticatedClient();
            graphService = new GraphService(graphClient);
            managedEventRepository = new ManagedEventRepository(graphClient);
            schemaExtensionsRepository = new SchemaExtensionsRepository(graphClient);
        }
        //// GET: Event
        //public async Task<ActionResult> Index()
        //{
        //    return await Index(String.Empty);
        //}

        [Authorize, HandleAdalException]
        public async Task<ActionResult> NavigateTo(string Id)
        {
            var url = await managedEventRepository.GetWebUrlForGroup(Id);
            return Redirect(url);
        }

        [Authorize, HandleAdalException]
        public async Task<ActionResult> Index(string SelectedEventTypeId)
        {
            bool success = Enum.TryParse<EventTypes>(SelectedEventTypeId, out EventTypes result);

            if (!success)
            {
                SelectedEventTypeId = "";
            }
            var viewModel = new EventsOverviewViewModel
            {
                ManagedEvents = await managedEventRepository.GetManagedEvents(SelectedEventTypeId),
                SelectedEventTypeId = SelectedEventTypeId
            };

            return View(viewModel);
        }

        [Authorize, HandleAdalException]
        public async Task<ActionResult> EventDetails(string EventId)
        {
            var accessTokenSharePoint = await AadHelper.GetAccessTokenForSharePoint();
            var repoSP = new VideoChannelRepository(accessTokenSharePoint);

            //OLD MOD tenant
            //string channelId = "037b317f-2fcf-451a-a618-99dfb45aa2f1";

            //TODO Retrieve from schema extension
            string channelId = "7710364b-6748-49ef-9a47-002240b994fd";

            var viewmodel = new EventDetailsViewModel
            {
                Event = await managedEventRepository.GetManagedEventById(EventId),
                Videos = await repoSP.GetChannelVideos(channelId),
                GroupCalendarEvents = await managedEventRepository.GetGroupCalendarEvents(EventId),
                BudgetStatus = await managedEventRepository.GetBudgetFromExcelsheet(EventId, SettingsHelper.drive_path_to_excelsheet),
                PlanningStatus = "Good"
            };

            return View(viewmodel);
        }

        [Authorize, HandleAdalException]
        public ActionResult Create()
        {
            var event1 = new Models.ManagedEvent();            
            var values = Enum.GetNames(typeof(Models.EventTypes));
            ViewBag.EventTypes = values.Select(v =>
                {
                    return new SelectListItem
                    {
                        Text = v,
                        Value = v
                    };
                });
            return View(event1);
        }

        [HttpPost, Authorize, HandleAdalException]
        public async Task<ActionResult> Create(Models.ManagedEvent event1)
        {         
            var managedEvent = await managedEventRepository.CreateManagedEvent(event1);
            return RedirectToAction("created", new { message = managedEvent.Title } );
        }

        [Authorize, HandleAdalException]
        public ActionResult Created(string message)
        {
            ViewBag.message = $"The following event has been successfully created: {message}";
            return View();
        }

        [Authorize, HandleAdalException]
        public async Task<ActionResult> FindMeetingTime(string id)
        {
            var meetingTimes = await managedEventRepository.FindMeetingTime();

            FindMeetingTimeViewModel model = new FindMeetingTimeViewModel()
            {
                MeetingTimeCandidates = meetingTimes
            };

            return View(model);

        }
    }
}