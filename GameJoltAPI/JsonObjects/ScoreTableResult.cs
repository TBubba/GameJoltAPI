using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace GameJoltAPI.JsonObjects
{
    [JsonObject()]
    public class ScoreTableResult
    {
        [JsonProperty("response")]
        public ScoreTableResponse Response { get; set; }
    }
}
