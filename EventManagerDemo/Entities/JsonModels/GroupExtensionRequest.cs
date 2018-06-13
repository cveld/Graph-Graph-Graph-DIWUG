using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventManagerDemo.Models.JsonHelpers
{
    public class GroupExtensionRequest
    {
        public string odatatype {
            get {
                return "Microsoft.Graph.OpenTypeExtension";
            }
        }
        public string extensionName
        {
            get
            {
                return "EventManagerDemo.GroupExtension";
            }
        }
        public string myvalue { get; set; }
    }

}