using EventManagerDemo.Utils;
using Microsoft.Graph;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using EventManagerDemo.Models.Mappers;
using EventManagerDemo.Models.JsonHelpers;

namespace EventManagerDemo.Models
{
    public class ManagedEventRepository
    {
        GraphServiceClient graphClient;

        public ManagedEventRepository(GraphServiceClient graphClient)
        {
            this.graphClient = graphClient;
        }

        internal async Task<ManagedEvent> CreateManagedEvent(ManagedEvent event1)
        {
            // https://developer.microsoft.com/en-us/graph/docs/api-reference/v1.0/api/group_post_groups

            var request = new BaseRequest($"{graphClient.BaseUrl}/groups", graphClient);
            request.Method = "POST";
            request.ContentType = "application/json";
            var requestMessage = event1.ToJson();
            var json_RequestMessage = JsonConvert.SerializeObject(requestMessage, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            var response = await request.SendRequestAsync(json_RequestMessage, CancellationToken.None).ConfigureAwait(continueOnCapturedContext: false);
            var resultString = response.Content.ReadAsStringAsync().Result;
            var json_ManagedEvent = JsonConvert.DeserializeObject<JsonHelpers.ManagedEvent>(resultString);
            return json_ManagedEvent.ToModel();
        }

        internal async Task<IEnumerable<ManagedEvent>> GetManagedEvents(string selectedEventId)
        {
            string requestUrl = $"{graphClient.BaseUrl}/groups?$select=id,mail,displayName,description,{extensionid}&$filter={extensionid}/ManagedEvent eq true";
            if (!string.IsNullOrEmpty(selectedEventId))
            {
                requestUrl += $" and {extensionid}/EventType eq '{selectedEventId}'";
            }
            var request = new BaseRequest(requestUrl, graphClient);
            var response = await request.SendRequestAsync(null, CancellationToken.None);
            var resultString = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<JsonHelpers.ManagedEventCollection>(resultString);

            if (result == null || result.value == null)
            {
                return new List<ManagedEvent>();
            }

            var query = result.value.Select(p => p.ToModel());

            return query;
        }

        internal async Task<ManagedEvent> GetManagedEventById(string eventId)
        {
            // We cannot use the Graph SDK fluent API here as we need to deserialize into the more specialized ManagedEventCollection:
            string requestUrl = $"{graphClient.BaseUrl}/groups/{eventId}?$select=id,displayName,description,{extensionid}";
            var request = new BaseRequest(requestUrl, graphClient);
            var response = await request.SendRequestAsync(null, CancellationToken.None);
            var resultString = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<JsonHelpers.ManagedEvent>(resultString);

            if (result == null)
            {
                return new ManagedEvent();
            }

            // Map the incoming json-data to our domain model:
            var model = result.ToModel();
            return model;

        }          

     
        /// <summary>
        /// Gets the WebUrl of a Group
        /// </summary>
        /// <remarks>Other way is to use the Mail property to assemble a ConversationsUrl. Refer to Group Json class</remarks>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<string> GetWebUrlForGroup(string id)
        {
            var request = new BaseRequest($"{graphClient.BaseUrl}/groups/{id}/sites/root?$select=webUrl", graphClient);
            var response = await request.SendRequestAsync(null, CancellationToken.None);
            var resultString = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<JsonHelpers.SitesRoot>(resultString);
            return result != null ? result.webUrl : null;
        }

        const string extensionid = ManagedEventSchemaExtension.schemaid; // "mseventmanager_EventManagerDemo";      

        internal async Task<List<GroupCalendarEvent>> GetGroupCalendarEvents(string selectedEventId)
        {

            var options = new List<Option>
            {
                new QueryOption("startDateTime", DateTime.Now.ToString("s")),
                new QueryOption("endDateTime", DateTime.Now.AddDays(7).ToString("s")),
            };

            var items = await graphClient.Groups[selectedEventId].CalendarView.Request(options)
                                    .OrderBy("start/dateTime")
                                    .Top(10)
                                    .GetAsync();


            if (items == null)
            {
                return new List<GroupCalendarEvent>();
            }

            List<GroupCalendarEvent> groupCalendarEvents = new List<GroupCalendarEvent>();

            //Create the Model objects
            foreach (var item in items.CurrentPage)
            {
                var groupcalendarEvent = new GroupCalendarEvent
                {
                    Id = item.Id,
                    Subject = item.Subject,
                    Body = new Body
                    {
                        Content = item.Body.Content
                    },
                    Start = new Start
                    {
                        DateTime = item.Start.DateTime,
                        TimeZone = item.Start.TimeZone
                    },
                    End = new End
                    {
                        DateTime = item.End.DateTime,
                        TimeZone = item.End.TimeZone
                    }
                };
                groupCalendarEvents.Add(groupcalendarEvent);
            }
            return groupCalendarEvents;
        }

        public async Task StoreSomethingIntoExcelsheet(string id, string path)
        {
            var filename = Path.GetFileName(path);

            // If you want to write to your excelsheet you have to work with a session
            var sessioninfo = await graphClient.Groups[id].Drive.Root.ItemWithPath(filename).Workbook.CreateSession(false).Request().PostAsync();

            // DO SOMETHING
            var result = await graphClient.Groups[id].Drive.Root.ItemWithPath(filename).Workbook.Names["Budget"].Range().Request().Header("workbook-session-id", sessioninfo.Id).GetAsync();

            // Close the ession
            await graphClient.Groups[id].Drive.Root.ItemWithPath(filename).Workbook.CloseSession().Request().Header("workbook-session-id", sessioninfo.Id).PostAsync();
        }

        public async Task<string> GetBudgetFromExcelsheet(string id, string path)
        {
            var filename = Path.GetFileName(path);
            var result = await graphClient.Groups[id].Drive.Root.ItemWithPath(filename).Workbook.Names["Budget"].Range().Request().GetAsync();
            var budget = result.Values.First.First.ToString();
            return budget;
        }

        public async Task<DriveItem> PutExcelsheet(string id, string local_path, string drive_path)
        {            
            DriveItem di;
            
            using (Stream source = System.IO.File.OpenRead(local_path))
            {
                // https://github.com/OneDrive/onedrive-sdk-csharp/blob/master/docs/items.md                
                di = await graphClient.Groups[id].Drive.Root.ItemWithPath(drive_path).Content.Request().PutAsync<DriveItem>(source);
            }

            return di;
        }

        public async Task<List<MeetingTimeCandidate>> FindMeetingTime()
        {
            // Get the group id.
            //var group = await graphClient.Groups[id].Request().GetAsync();

            User me = await graphClient.Me.Request().Select("mail,userPrincipalName").GetAsync();


            //Build the list of attendees
            List<Attendee> attendees = new List<Attendee>();

            Attendee attendee = new Attendee();
            EmailAddress mailaddress1 = new EmailAddress();
            mailaddress1.Address = me.Mail;
            attendee.EmailAddress = mailaddress1;
            attendees.Add(attendee);

            //Build the list of locationcontraints
            LocationConstraint location = new LocationConstraint();
            location.IsRequired = false;
            location.SuggestLocation = false;

            List<LocationConstraintItem> locationConstraints = new List<LocationConstraintItem>();
            locationConstraints.Add(new LocationConstraintItem { DisplayName = "", LocationEmailAddress = "conf32room1368@mod498161.onmicrosoft.com" });

            //Build the duration
            Duration duration = new Duration("PT1H");
            TimeConstraint timeConstraint = new TimeConstraint();
            List<TimeSlot> timeSlots = new List<TimeSlot>();
            TimeSlot slot1 = new TimeSlot();
            Microsoft.Graph.DateTimeTimeZone start = new Microsoft.Graph.DateTimeTimeZone();
            start.DateTime = @"2017-06-10T15:30:00.000";
            start.TimeZone = @"W. Europe Standard Time";

            Microsoft.Graph.DateTimeTimeZone end = new Microsoft.Graph.DateTimeTimeZone();
            end.DateTime = @"2017-06-14T18:00:00.000";
            end.TimeZone = @"W. Europe Standard Time";
            slot1.Start = start;
            slot1.End = end;

            timeSlots.Add(slot1);
            timeConstraint.Timeslots = timeSlots;

            //Execute the request
            var request = graphClient.Me.FindMeetingTimes(attendees, location, timeConstraint, duration, 3, false,false, 50).Request();
            MeetingTimeSuggestionsResult meetingSuggestions = await request.PostAsync();

            List<MeetingTimeCandidate> meetingTimeCandidates = new List<MeetingTimeCandidate>();

            // Create model objects
            foreach (MeetingTimeSuggestion meetingJson in meetingSuggestions.MeetingTimeSuggestions)
            {
                MeetingTimeCandidate candidate = new MeetingTimeCandidate
                {
                    Confidence = meetingJson.Confidence.Value,
                    MeetingTimeSlotStart = meetingJson.MeetingTimeSlot.Start.DateTime,
                    MeetingTimeSlotEnd = meetingJson.MeetingTimeSlot.End.DateTime,
                    OrganizerAvailability = meetingJson.OrganizerAvailability.Value.ToString() ?? string.Empty,
                    SuggestionHint = meetingJson.SuggestionReason
                };
                meetingTimeCandidates.Add(candidate);
            }

            return meetingTimeCandidates;


        }
    }

}