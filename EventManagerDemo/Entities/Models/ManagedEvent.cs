using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventManagerDemo.Models
{
    /// <summary>
    /// ManagedEvent is an extended Graph Group object
    /// </summary>
    public class ManagedEvent : Group
    {        
        public string Title { get; set; }
        public DateTime? EventDate { get; set; }
        public string EventManager { get; set; }
        public string Confidentiality { get; set; }
        public EventTypes? EventType { get; set; }        
        public string[] GroupMembers { get; set; }          
    }

    public enum EventTypes
    {
        Podcast,
        Webcast,
        SPSaturday,
        FullConference
    }
}