﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace GameJoltAPI.JsonObjects
{
    [JsonObject()]
    public class ScoreResponse
    {
        [JsonProperty("success")]
        public string Success { get; set; }

        [JsonProperty("scores")]
        public List<ScoreJson> Scores { get; set; }
    }
}
