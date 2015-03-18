using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace GameJoltAPI.JsonObjects
{
    [JsonObject()]
    public class DataKeysResponse
    {
        [JsonProperty("success")]
        public string Success { get; set; }

        [JsonProperty("keys")]
        public List<DataKey> Keys { get; set; }
    }
}
