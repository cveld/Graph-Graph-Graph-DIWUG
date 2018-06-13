using EventManagerDemo.Utils;
using Microsoft.Graph;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using EventManagerDemo.Entities.JsonModels;

namespace EventManagerDemo.Models
{
    /// <summary>
    /// TO DO: This class does not yet utilize the Graph SDK fully
    /// </summary>
    public class GroupsRepository
    {
        //string accessToken;
        GraphServiceClient graphClient;
        public GroupsRepository(GraphServiceClient graphClient)
        {
            //this.accessToken = accessToken;
            this.graphClient = graphClient;
        }

        /// <summary>
        /// Example method that showcases how to use the Graph SDK for creating a Office365 Group
        /// </summary>
        /// <param name="graphClient"></param>
        /// <param name="event1"></param>
        /// <returns></returns>
        public async Task<Microsoft.Graph.Group> CreateGroup()
        {
            // https://developer.microsoft.com/en-us/graph/docs/api-reference/v1.0/api/group_post_groups
            var group = await graphClient.Groups.Request().AddAsync(new Microsoft.Graph.Group
            {
                DisplayName = "My displayname",
                Description = "My description",
                MailEnabled = true,
                SecurityEnabled = false,
                MailNickname = "mynickname",
                GroupTypes = new string[] { "unified" }  // magic string for Office365 groups
            });

            return group;
        }

        public async Task<IEnumerable<Group>> GetGroups()
        {
            
            var request = new BaseRequest($"{graphClient.BaseUrl}/me/memberOf/$/microsoft.graph.group", graphClient);
            //https://graph.microsoft.com/v1.0/me/memberOf/$/microsoft.graph.group?$filter=groupTypes/any(a:a%20eq%20'unified')
            //request.QueryOptions.Add(new QueryOption("$filter", "groupTypes/any(a:a%20eq%20'unified')"));

            request.ContentType = "application/json";           
            var response = await request.SendRequestAsync(null, CancellationToken.None).ConfigureAwait(continueOnCapturedContext: false);            
            var resultString = response.Content.ReadAsStringAsync().Result;            
            var results = JsonConvert.DeserializeObject<GroupMemberOfCollectionWithReferencesResponse<Group>>(resultString);

            var groups = new List<Group>();
            foreach (var jsonGroup in results.Value)            
            {
                if (jsonGroup.SecurityEnabled)
                {
                    var domainGroup = new Group
                    {
                        Id = jsonGroup.Id,
                        DisplayName = jsonGroup.DisplayName,
                        Description = jsonGroup.Description,
                        Visibility = jsonGroup.Visibility,
                        Mail = jsonGroup.Mail
                    };
                    groups.Add(domainGroup);
                }
            }
                                                                
            return groups;
        }
      
        public async Task<byte[]> GetGroupPhoto(string groupid)
        {
            var imageStream = await graphClient.Groups[groupid].Photo.Content.Request().GetAsync();                                       
            byte[] bytes = new byte[imageStream.Length];
            imageStream.Read(bytes, 0, (int)imageStream.Length);
            return bytes;            
        } 
    }
}