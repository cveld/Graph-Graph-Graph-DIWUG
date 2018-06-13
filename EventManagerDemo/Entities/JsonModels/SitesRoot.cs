using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventManagerDemo.Models.JsonHelpers
{
    public class SitesRoot
    {
        public string odatacontext { get; set; }
        public DateTime createdDateTime { get; set; }
        public string description { get; set; }
        public string id { get; set; }
        public DateTime lastModifiedDateTime { get; set; }
        public string name { get; set; }
        public string webUrl { get; set; }            
        public Sitecollection siteCollection { get; set; }
        public string displayName { get; set; }
    }
        
    public class Sitecollection
    {
        public string hostname { get; set; }
    }
    
}