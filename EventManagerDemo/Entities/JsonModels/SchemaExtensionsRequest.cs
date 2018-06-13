using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventManagerDemo.Models.JsonHelpers
{
    public class SchemaExtensionsRequest
    {    
        public string id { get; set; }
        public string description { get; set; }
        public string status { get; set; }
        public string[] targetTypes { get; set; }
        public Property1[] properties { get; set; }
    }

    public class Property1
    {
        public string name { get; set; }
        public string type { get; set; }
    }

}