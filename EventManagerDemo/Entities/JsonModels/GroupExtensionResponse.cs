using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventManagerDemo.Models.JsonHelpers
{
    public class GroupExtensionResponse
    {   
        public string odatacontext { get; set; }
        public string odatatype { get; set; }
        public string odataid { get; set; }
        public string id { get; set; }
        public string extensionName { get; set; }
        public string myvalue { get; set; }
    }

}