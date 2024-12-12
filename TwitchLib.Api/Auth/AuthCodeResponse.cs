﻿using Newtonsoft.Json;

namespace TwitchLib.Api.Auth
{
    /// <summary>
    /// Auth Code Response
    /// </summary>
    public class AuthCodeResponse
    {
        /// <summary>
        /// Access Token
        /// </summary>
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; protected set; }

        /// <summary>
        /// Refresh Token
        /// </summary>
        [JsonProperty(PropertyName = "refresh_token")]
        public string RefreshToken { get; protected set; }

        /// <summary>
        /// Expires In
        /// </summary>
        [JsonProperty(PropertyName = "expires_in")]
        public int ExpiresIn { get; protected set; }

        /// <summary>
        /// Scopes
        /// </summary>
        [JsonProperty(PropertyName = "scope")]
        public string[] Scopes { get; protected set; }

        /// <summary>
        /// Token Type
        /// </summary>
        [JsonProperty(PropertyName = "token_type")]
        public string TokenType { get; set; }
    }
}