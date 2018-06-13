using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventManagerDemo.Models
{
    public class GroupCalendarEvent
    {

        public string Id { get; set; }
        public string Subject { get; set; }
        public Body Body { get; set; }
        public Start Start { get; set; }
        public End End { get; set; }
    }

    public class Body
    {
        public string ContentType { get; set; }
        public string Content { get; set; }
    }

    public class Start
    {
        public string DateTime { get; set; }
        public string TimeZone { get; set; }
    }

    public class End
    {
        public string DateTime { get; set; }
        public string TimeZone { get; set; }
    }
}