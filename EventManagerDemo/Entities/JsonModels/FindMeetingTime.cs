using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventManagerDemo.Models.JsonHelpers
{
    public class EmailAddressModel
    {
        public string Address { get; set; }
    }

    public class AttendeeModelModel
    {
        public string Type { get; set; }
        public EmailAddressModel EmailAddress { get; set; }
    }

    public class AddressModel
    {
        public string Type { get; set; }
        public string PostOfficeBox { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string CountryOrRegion { get; set; }
        public string PostalCode { get; set; }
    }

    public class CoordinatesModel
    {
        public int Altitude { get; set; }
        public int Latitude { get; set; }
        public int Longitude { get; set; }
        public int Accuracy { get; set; }
        public int AltitudeAccuracy { get; set; }
    }

    public class LocationModel
    {
        public string DisplayName { get; set; }
        public AddressModel Address { get; set; }
        public CoordinatesModel Coordinates { get; set; }
    }

    public class LocationConstraintModel
    {
        public bool IsRequired { get; set; }
        public bool SuggestLocation { get; set; }
        public List<LocationModel> Locations { get; set; }
    }

    public class DateTimeValueModel
    {
        public string Date { get; set; }
        public string Time { get; set; }
        public string TimeZone { get; set; }
    }

    public class TimeSlotModel
    {
        public DateTimeValueModel Start { get; set; }
        public DateTimeValueModel End { get; set; }
    }

    public class TimeConstraintModel
    {
        public List<TimeSlotModel> TimeSlots { get; set; }
    }

    public class AttendeeAvailabilityModel
    {
        public AttendeeModelModel Attendee { get; set; }
        public string Availability { get; set; }
    }

    public class MeetingTimeSlotModel
    {
        public DateTimeValueModel Start { get; set; }
        public DateTimeValueModel End { get; set; }
    }

    public class MeetingTimeCandidateModel
    {
        public MeetingTimeSlotModel MeetingTimeSlot { get; set; }
        public double Confidence { get; set; }
        public string OrganizerAvailability { get; set; }
        public List<AttendeeAvailabilityModel> AttendeeAvailability { get; set; }
        public List<LocationModel> Locations { get; set; }
        public string SuggestionHint { get; set; }
    }
}