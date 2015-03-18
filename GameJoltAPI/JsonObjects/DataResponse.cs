using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace GameJoltAPI.JsonObjects
{
    [JsonObject()]
    public class DataResponse // This should not ever be used
    {
        [JsonProperty("success")]
        public string Success { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }
    }
}
