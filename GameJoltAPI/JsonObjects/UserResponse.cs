﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace GameJoltAPI.JsonObjects
{
    [JsonObject()]
    public class UserResponse
    {
        [JsonProperty("success")]
        public string Success { get; set; }

        [JsonProperty("users")]
        public List<UserJson> Users { get; set; }
    }
}
