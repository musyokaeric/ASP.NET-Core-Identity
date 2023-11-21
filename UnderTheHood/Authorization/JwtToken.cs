﻿using Newtonsoft.Json;

namespace UnderTheHood.Authorization
{
    public class JwtToken
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; } = string.Empty;
        [JsonProperty("expires_at")]
        public string ExpiresAt { get; set; } = string.Empty;
    }
}
