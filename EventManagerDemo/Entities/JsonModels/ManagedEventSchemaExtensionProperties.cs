using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventManagerDemo.Models.JsonHelpers
{
    public class ManagedEventSchemaExtensionProperties
    {
        static public IEnumerable<ExtensionSchemaProperty> Get()
        {
            return new ExtensionSchemaProperty[] {
                new ExtensionSchemaProperty
                {
                    Name = "managedEvent",
                    Type = "Boolean",
                },
                new ExtensionSchemaProperty
                {
                    Name = "eventDate",
                    Type = "DateTime",
                },
                new ExtensionSchemaProperty
                {
                    Name = "eventType",
                    Type = "String"
                },
                new ExtensionSchemaProperty
                {
                    Name = "mykey",
                    Type = "String"
                }
            };
        }
    }
}