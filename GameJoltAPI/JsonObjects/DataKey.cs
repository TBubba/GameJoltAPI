using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace GameJoltAPI.JsonObjects
{
    [JsonObject()]
    public class DataKey
    {
        [JsonProperty("key")]
        public string Key { get; set; }
    }
}
