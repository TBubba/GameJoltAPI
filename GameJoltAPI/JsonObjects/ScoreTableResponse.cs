using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace GameJoltAPI.JsonObjects
{
    [JsonObject()]
    public class ScoreTableResponse
    {
        [JsonProperty("success")]
        public string Success { get; set; }

        [JsonProperty("tables")]
        public List<ScoreTableJson> Tables { get; set; }
    }
}
