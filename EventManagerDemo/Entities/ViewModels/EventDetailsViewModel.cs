using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EventManagerDemo.Models
{
    public class EventDetailsViewModel
    {
        public EventDetailsViewModel()
        {

        }
        public ManagedEvent Event { get; set; }

        public string PlanningStatus { get; set; }
        public string BudgetStatus { get; set; }

        public List<GroupCalendarEvent> GroupCalendarEvents { get; set; }

        public List<Video> Videos { get; set; }

    }
}