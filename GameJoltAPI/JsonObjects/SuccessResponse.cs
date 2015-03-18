using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace GameJoltAPI.JsonObjects
{
    [JsonObject()]
    public class SuccessResponse
    {
        [JsonProperty("success")]
        public string Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
