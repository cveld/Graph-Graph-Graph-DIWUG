using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventManagerDemo.Models.JsonHelpers
{
    public class FindMeetingTimeRequestModel
    {
        public List<AttendeeModelModel> Attendees { get; set; }
        public LocationConstraintModel LocationConstraint { get; set; }
        public TimeConstraintModel TimeConstraint { get; set; }
        public string MeetingDuration { get; set; }
        public int MaxCandidates { get; set; }
        public bool IsOrganizerOptional { get; set; }
        public bool ReturnSuggestionHints { get; set; }
    }
}