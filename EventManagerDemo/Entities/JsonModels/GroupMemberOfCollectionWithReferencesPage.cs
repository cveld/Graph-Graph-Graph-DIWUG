using Microsoft.Graph;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventManagerDemo.Entities.JsonModels
{
    // copied from the Graph SDK source code GroupMemberOfCollectionWithReferencesPage.cs and extended it with a generic type parameter
    public partial class GroupMemberOfCollectionWithReferencesPage<T> : CollectionPage<T>
    {
        /// <summary>
        /// Gets the next page <see cref="IGroupMemberOfCollectionWithReferencesRequest"/> instance.
        /// </summary>
        public IGroupMemberOfCollectionWithReferencesRequest NextPageRequest { get; private set; }

        /// <summary>
        /// Initializes the NextPageRequest property.
        /// </summary>
        public void InitializeNextPageRequest(IBaseClient client, string nextPageLinkString)
        {
            if (!string.IsNullOrEmpty(nextPageLinkString))
            {
                this.NextPageRequest = new GroupMemberOfCollectionWithReferencesRequest(
                    nextPageLinkString,
                    client,
                    null);
            }
        }
    }

    

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class GroupMemberOfCollectionWithReferencesResponse<T>
    {
        /// <summary>
        /// Gets or sets the <see cref="IGroupMemberOfCollectionWithReferencesPage"/> value.
        /// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "value", Required = Newtonsoft.Json.Required.Default)]
        public GroupMemberOfCollectionWithReferencesPage<T> Value { get; set; }

        /// <summary>
        /// Gets or sets additional data.
        /// </summary>
        [JsonExtensionData(ReadData = true)]
        public IDictionary<string, object> AdditionalData { get; set; }
    }
}