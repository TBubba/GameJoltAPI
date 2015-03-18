using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace GameJoltAPI.JsonObjects
{
    [JsonObject()]
    public class ScoreJson
    {
        [JsonProperty("score")]
        public string Score { get; set; }

        [JsonProperty("sort")]
        public string Sort { get; set; }

        [JsonProperty("extra_data")]
        public string ExtraData { get; set; }

        [JsonProperty("user")]
        public string User { get; set; }

        [JsonProperty("user_id")]
        public string UserID { get; set; }

        [JsonProperty("guest")]
        public string Guest { get; set; }

        [JsonProperty("stored")]
        public string Stored { get; set; }
    }
}
