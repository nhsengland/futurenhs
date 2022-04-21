﻿using Newtonsoft.Json;

namespace FutureNHS.Api.Models.Content
{
    public class ContentModel
    {
        [JsonProperty("item")]
        public ItemModel? Item { get; set; }

        [JsonProperty("content")]
        public Dictionary<string, object>? Content { get; set; }
    }
}
