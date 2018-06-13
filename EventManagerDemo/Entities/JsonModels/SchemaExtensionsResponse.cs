using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventManagerDemo.Models.JsonHelpers
{
    public class SchemaExtensionsResponse
    {    
        public string id { get; set; }
        public string description { get; set; }
        public string[] targetTypes { get; set; }
        public string status { get; set; }
        public string owner { get; set; }
        public Property1[] properties { get; set; }
    }

    
}