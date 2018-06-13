using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace EventManagerDemo.Models.JsonHelpers
{
    public class GroupCalendarEventCollection<T>
    {
        public string odatacontext { get; set; }
        public T[] value { get; set; }
    }

    public class GroupCalendarEvent
    {
        public string odataetag { get; set; }
        public string id { get; set; }
        public string subject { get; set; }
        public Body body { get; set; }
        public Start start { get; set; }
        public End end { get; set; }
    }

    public class Body
    {
        public string contentType { get; set; }
        public string content { get; set; }
    }

    public class Start
    {
        public DateTime dateTime { get; set; }
        public string timeZone { get; set; }
    }

    public class End
    {
        public DateTime dateTime { get; set; }
        public string timeZone { get; set; }
    }
}