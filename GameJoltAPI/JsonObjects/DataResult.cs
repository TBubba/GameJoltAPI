using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace GameJoltAPI.JsonObjects
{
    [JsonObject()]
    public class DataResult // This should not ever be used
    {
        [JsonProperty("response")]
        public DataResponse Response { get; set; }
    }
}
