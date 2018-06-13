using EventManagerDemo.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventManagerDemo.Models.JsonHelpers
{
    public class ManagedEventCollection : GraphCollection<ManagedEvent>
    {
    }

    public class ManagedEvent : Microsoft.Graph.Group
    {
        // The Json Serializer will use the name of this property to build up the schema extension property in the resulting json
        // Therefore, the name must exactly match with the name of the registered schema extension.
        [JsonProperty(ManagedEventSchemaExtension.schemaid)]
        public MsEventManager_GroupExtensions mseventmanager_EventManagerDemo { get; set; }

    }

    public class MsEventManager_GroupExtensions
    {
        public bool managedEvent { get; set; }
        public DateTime? eventDate { get; set; }
        public string eventType { get; set; }
    }

}