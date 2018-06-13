using EventManagerDemo.Models.JsonHelpers;
using Microsoft.Graph;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace EventManagerDemo.Models
{
    public class SchemaExtensionsRepository
    {
        GraphServiceClient graphClient;
        //SchemaExtensionsService schemaExtensionService;

        public SchemaExtensionsRepository(GraphServiceClient graphClient)
        {
            this.graphClient = graphClient;
            //this.schemaExtensionService = new SchemaExtensionsService(graphClient);
        }

        /// <summary>
        /// Get all schema extensions
        /// </summary>
        /// <param name="graphClient"></param>
        /// <returns></returns>
        internal async Task<IEnumerable<SchemaExtension>> SchemaExtensions()
        {
            var result = await graphClient.SchemaExtensions.Request().GetAsync();
            return result;
        }

        public async Task<bool> IsSchemaExtensionPresent(SchemaExtensionTypes types)
        {
            try
            {
                var response = await graphClient.SchemaExtensions[GetSchemaExtensionId(types)].Request().GetAsync();
                return true;
            }
            catch (ServiceException se)
            {
                if (se.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return false;
                }

                throw se;
            }
        }

        public string ToSchemaExtensionType(JSchema property)
        {
            var typeWithoutNull = property.Type & ~JSchemaType.Null;
            switch (typeWithoutNull)
            {                
                case JSchemaType.String:
                    if (property.Format == "date-time")
                    {
                        return "DateTime";
                    }
                    else
                    {
                        return "String";
                    }                                    
                case JSchemaType.Boolean:
                    return "Boolean";
                case JSchemaType.Integer:
                case JSchemaType.Number:
                    return "Integer";
                case JSchemaType.Array:
                    // TO DO; detect a byte array; this can be translated to a Binary type (max 256 bytes)
                    throw new NotSupportedException("Array is not supported in a Graph Schema Extension");
                default:
                    throw new NotSupportedException($"{property.Type} is not supported in a Graph Schema Extension");
            }
        }

        public IEnumerable<ExtensionSchemaProperty> ToSchemaExtensionPropertiesCollection(Type type)
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(type);
            List<ExtensionSchemaProperty> properties = new List<ExtensionSchemaProperty>();
            foreach (var property in schema.Properties)
            {

                var newProperty = new ExtensionSchemaProperty
                {
                    Name = property.Key,
                    Type = ToSchemaExtensionType(property.Value)
                };
                properties.Add(newProperty);
            }
            return properties;
        }

        internal async Task<SchemaExtension> AddTestProjectSchemaExtension(SchemaExtensionTypes types)
        {
            var schemaExtension = SchemaExtensionBuilder(types);
            var result = await graphClient.SchemaExtensions.Request().AddAsync(schemaExtension);
            return result;
        }

        public string GetSchemaExtensionId(SchemaExtensionTypes types)
        {
            switch (types)
            {
                case SchemaExtensionTypes.ManagedEvent:
                    return ManagedEventSchemaExtension.schemaid;
                case SchemaExtensionTypes.TestExtension:
                    return TestExtensionSchemaExtension.schemaid;
            }

            throw new ArgumentException(nameof(types));
        }

        Microsoft.Graph.SchemaExtension SchemaExtensionBuilder(SchemaExtensionTypes types)
        {
            switch (types)
            {
                case SchemaExtensionTypes.ManagedEvent:
                    return new Microsoft.Graph.SchemaExtension
                    {
                        Id = GetSchemaExtensionId(types),
                        Description = "Extensions for EventManagerDemo",
                        TargetTypes = new string[] { "Group" },
                        Properties = JsonHelpers.ManagedEventSchemaExtensionProperties.Get()        // TO DO: infer the schema from a POCO JsonClass
                    };
                case SchemaExtensionTypes.TestExtension:
                    return new Microsoft.Graph.SchemaExtension
                    {
                        Id = GetSchemaExtensionId(types),
                        Description = "Extension for EventManagerDemo",
                        TargetTypes = new string[] { "Group" },
                        Properties = ToSchemaExtensionPropertiesCollection(typeof(JsonHelpers.TestExtension))
                    };

            }

            throw new ArgumentException(nameof(types));
        }

        public async Task<SchemaExtension> AddSchemaExtension(SchemaExtensionTypes types)
        {
            var schemaExtension = SchemaExtensionBuilder(types);
            return await graphClient.SchemaExtensions.Request().AddAsync(schemaExtension);
        }
               
        public async Task<SchemaExtension> SetSchemaExtensionStatus(SchemaExtensionTypes types, JsonHelpers.SchemaExtensionStatusTypes status) 
        {
            var schemaExtension = new SchemaExtension
            {
                Id = GetSchemaExtensionId(types),
                Status = status.ToString()
            };

            var result = await graphClient.SchemaExtensions[schemaExtension.Id].Request().UpdateAsync(schemaExtension);
            return result;
        }

        internal async Task<SchemaExtension> SetSchemaExtensionStatusAvailable(SchemaExtensionTypes types)
        {
            var result = await SetSchemaExtensionStatus(types, JsonHelpers.SchemaExtensionStatusTypes.Available);
            return result;
        }

        public async Task<SchemaExtension> SetSchemaExtensionStatusInDevelopment(SchemaExtensionTypes types)
        {
            var result = await SetSchemaExtensionStatus(types, JsonHelpers.SchemaExtensionStatusTypes.InDevelopment);
            return result;
        }

        public async Task<SchemaExtension> SetSchemaExtensionStatusDeprecated(SchemaExtensionTypes types)
        {
            var result = await SetSchemaExtensionStatus(types, JsonHelpers.SchemaExtensionStatusTypes.Deprecated);
            return result;
        }

        public async Task DeleteSchemaExtension(SchemaExtensionTypes types)
        {
            await graphClient.SchemaExtensions[GetSchemaExtensionId(types)].Request().DeleteAsync().ConfigureAwait(continueOnCapturedContext: false);
        }

        internal async Task UpdateManagedEventSchemaExtension()
        {
            //ExtensionSchemaProperty

            var schemaExtension = SchemaExtensionBuilder(SchemaExtensionTypes.ManagedEvent);
            schemaExtension.Status = JsonHelpers.SchemaExtensionStatusTypes.Available.ToString();

            await graphClient.SchemaExtensions[schemaExtension.Id].Request().UpdateAsync(schemaExtension).ConfigureAwait(continueOnCapturedContext: false);
        }

    }
}