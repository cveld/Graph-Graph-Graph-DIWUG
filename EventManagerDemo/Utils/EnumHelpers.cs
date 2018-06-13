using EventManagerDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventManagerDemo.Utils
{
    public class EnumHelpers
    {
        static public EventTypes? EventTypeFromStringOrNull(string input)
        {
            bool success = Enum.TryParse(input, out EventTypes EventType);
            if (success)
            {
                return EventType;
            }

            return null;
        }
    }
}