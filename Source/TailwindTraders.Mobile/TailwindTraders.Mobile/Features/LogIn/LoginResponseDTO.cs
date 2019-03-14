using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TailwindTraders.Mobile.Features.LogIn
{
    public class LoginResponseDTO
    {
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }

        [JsonProperty(PropertyName = "token_type")]
        public string TokenType { get; set; }

        [JsonProperty(PropertyName = "expires_in")]
        public int ExpiresIn { get; set; }
    }
}
