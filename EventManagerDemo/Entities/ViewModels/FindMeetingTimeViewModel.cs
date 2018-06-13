using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventManagerDemo.Models
{
    public class FindMeetingTimeViewModel
    {
        public FindMeetingTimeViewModel()
        {

        }

        public List<MeetingTimeCandidate> MeetingTimeCandidates { get; set; }


    }
}