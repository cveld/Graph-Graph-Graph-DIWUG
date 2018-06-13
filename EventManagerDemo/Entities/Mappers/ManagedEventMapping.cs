using EventManagerDemo.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventManagerDemo.Models.Mappers
{
    static public class ManagedEventMapping
    {
        static public Models.ManagedEvent ToModel(this JsonHelpers.ManagedEvent managedEvent)
        {
            return new Models.ManagedEvent
            {
                Id = managedEvent.Id,
                EventDate = managedEvent.mseventmanager_EventManagerDemo != null ? managedEvent.mseventmanager_EventManagerDemo.eventDate : null,
                EventType = managedEvent.mseventmanager_EventManagerDemo != null ? EnumHelpers.EventTypeFromStringOrNull(managedEvent.mseventmanager_EventManagerDemo.eventType) : null,
                Title = managedEvent.DisplayName,
                Description = managedEvent.Description,
                Mail = managedEvent.Mail
            };
        }

        static public JsonHelpers.ManagedEvent ToJson(this Models.ManagedEvent event1)
        {
            var json_managedEvent = new JsonHelpers.ManagedEvent
            {
                Visibility = "Public",  // "Private"
                DisplayName = event1.Title,
                Description = event1.Description,
                MailEnabled = true,
                SecurityEnabled = false,
                MailNickname = event1.Title.Replace(" ", string.Empty),
                GroupTypes = new string[] { "unified" },  // magic string for Office365 groups
                mseventmanager_EventManagerDemo = new JsonHelpers.MsEventManager_GroupExtensions
                {
                    managedEvent = true,
                    eventDate = event1.EventDate,
                    eventType = event1.EventType.ToString(),

                }
            };

            return json_managedEvent;
        }


    }
}