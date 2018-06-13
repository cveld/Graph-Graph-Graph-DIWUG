using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EventManagerDemo.Models.JsonHelpers;

namespace EventManagerDemo.Models
{
    public class VideoListViewModel
    {
        public string ChannelId { get; set; }
        public string ChannelTitle { get; set; }
        public IEnumerable<Video> Videos { get; set; }
        public IEnumerable<Group> Groups { get; set; }
    }
}