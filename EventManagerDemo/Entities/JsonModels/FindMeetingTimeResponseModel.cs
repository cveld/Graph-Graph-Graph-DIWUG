using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace EventManagerDemo.Models.JsonHelpers
{
    public class FindMeetingTimeResponseModel
    {
        [JsonProperty("value")]
        public List<MeetingTimeCandidateModel> Candidates { get; set; }
    }
}