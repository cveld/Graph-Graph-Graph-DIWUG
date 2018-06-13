using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EventManagerDemo.Models
{
    public class EventsOverviewViewModel
    {
        public EventsOverviewViewModel()
        {

        }
        public IEnumerable<ManagedEvent> ManagedEvents { get; set; }
        [Display(Name = "Event type")]
        public string SelectedEventTypeId { get; set; }
        public IEnumerable<SelectListItem> EventTypes {
            get
            {
                var values = Enum.GetNames(typeof(Models.EventTypes));
                yield return new SelectListItem
                {
                    Text = "All"
                };

                foreach (var v in values)
                {
                    yield return new SelectListItem
                    {
                        Text = v,
                        Value = v
                    };
                }
            }
        }
    }

}