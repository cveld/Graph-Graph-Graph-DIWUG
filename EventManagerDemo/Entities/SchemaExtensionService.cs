using Microsoft.Graph;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace EventManagerDemo.Models
{
    public class SchemaExtensionsService
    {
        GraphServiceClient graphClient;

        public SchemaExtensionsService(GraphServiceClient graphClient)
        {
            this.graphClient = graphClient;
        }

        public async Task<T> UpdateAsync<T>(T item) where T : ISchemaExtension
        {
            var request = new BaseRequest($"{graphClient.BaseUrl}/schemaExtensions/{item.id}", graphClient);
            request.Method = "PATCH";
            request.ContentType = "application/json";
            var response = await request.SendRequestAsync(item, CancellationToken.None);
            //var response = await request.SendRequestAsync("{status:\"Deprecated\"}", CancellationToken.None);
            var resultString = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<T>(resultString);
            return result;
        }

        public async Task<T> AddAsync<T>(T item) where T : ISchemaExtension
        {
            var request = new BaseRequest($"{graphClient.BaseUrl}/schemaExtensions/{item.id}", graphClient);
            request.Method = "POST";
            request.ContentType = "application/json";

            var json_RequestMessage = JsonConvert.SerializeObject(item, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            var response = await request.SendRequestAsync(json_RequestMessage, CancellationToken.None);
            var resultString = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<T>(resultString);
            return result;
        }

        public async Task DeleteAsync<T>(T item) where T : ISchemaExtension
        {
            var request = new BaseRequest($"{graphClient.BaseUrl}/schemaExtensions/{item.id}", graphClient);
            request.Method = "DELETE";
            var response = await request.SendRequestAsync(null, CancellationToken.None);
        }

        public async Task<JObject> List()
        {
            var request = new BaseRequest($"{graphClient.BaseUrl}/schemaExtensions", graphClient);
            var response = await request.SendRequestAsync(null, CancellationToken.None);
            var resultString = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject(resultString);
            return (JObject)result;
        }
    }
}