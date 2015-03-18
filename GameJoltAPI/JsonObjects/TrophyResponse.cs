using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace GameJoltAPI.JsonObjects
{
    [JsonObject()]
    public class TrophyResponse
    {
        [JsonProperty("success")]
        public string Success { get; set; }

        [JsonProperty("trophies")]
        public List<TrophyJson> Trophies { get; set; }
    }
}
