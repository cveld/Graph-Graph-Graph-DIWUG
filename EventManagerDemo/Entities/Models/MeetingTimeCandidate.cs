using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventManagerDemo.Models
{
    public class MeetingTimeCandidate
    {
        public string MeetingTimeSlotStart { get; set; }
        public string MeetingTimeSlotEnd { get; set; }
        public double? Confidence { get; set; }
        public string OrganizerAvailability { get; set; }
        public string AttendeeEmail { get; set; }
        public double AttendeeAvailability { get; set; }
        public string LocationDisplayName { get; set; }
        public string SuggestionHint { get; set; }
    }
}