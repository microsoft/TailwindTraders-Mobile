using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TailwindTraders.Mobile.Features.LogIn
{
    public class TokenRequestDTO
    {
        public string Username { get; set; }

        public string Password { get; set; }

        [JsonProperty(PropertyName = "grant_type")]
        public string GrantType { get; set; }
    }
}
