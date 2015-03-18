using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace GameJoltAPI.JsonObjects
{
    [JsonObject()]
    public class UserJson
    {
        /// <summary>
        /// The ID of the user
        /// </summary>
        [JsonProperty("id")]
        public string ID { get; set; }

        /// <summary>
        /// Can be "User", "Developer", "Moderator", or "Administrator"
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// The user's username
        /// </summary>
        [JsonProperty("username")]
        public string Name { get; set; }

        /// <summary>
        /// The URL of the user's avatar
        /// </summary>
        [JsonProperty("avatar_url")]
        public string AvatarURL { get; set; }

        /// <summary>
        /// How long ago the user signed up
        /// </summary>
        [JsonProperty("signed_up")]
        public string SignedUp { get; set; }

        /// <summary>
        /// How long ago the user was last logged in. Will be "Online Now" if the user is currently online
        /// </summary>
        [JsonProperty("last_logged_in")]
        public string LastLogin { get; set; }

        /// <summary>
        /// "Active" if the user is still a member on the site. "Banned" if they've been banned
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        // The properties below will ONLY exist if the user is a developer

        /// <summary>
        /// This holds a value that is true if the user has values for the developer properties and is false otherwise.
        /// </summary>
        public bool IsDeveloper { get { return DeveloperDescription != null && DeveloperWebsite != null && DeveloperName != null; } }

        /// <summary>
        /// The Developers name
        /// </summary>
        [JsonProperty("developer_name")]
        public string DeveloperName { get; set; }

        /// <summary>
        /// The developers website, if they have one
        /// </summary>
        [JsonProperty("developer_website")]
        public string DeveloperWebsite { get; set; }

        /// <summary>
        /// The developers description of themselves. Formatting is removed from this result.
        /// </summary>
        [JsonProperty("deverloper_description")]
        public string DeveloperDescription { get; set; }
    }
}
