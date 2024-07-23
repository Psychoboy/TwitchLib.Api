﻿using Newtonsoft.Json;

namespace TwitchLib.Api.Helix.Models.Channels.GetChannelFollowers;

/// <summary>
/// A user that follow the specified broadcaster. 
/// </summary>
public class ChannelFollower
{
    /// <summary>
    /// An ID that uniquely identifies the user that’s following the broadcaster.
    /// </summary>
    [JsonProperty(PropertyName = "user_id")]
    public string UserId { get; protected set; }
    
    /// <summary>
    /// The user’s login name.
    /// </summary>
    [JsonProperty(PropertyName = "user_login")]
    public string UserLogin { get; protected set; }
    
    /// <summary>
    /// The user’s display name.
    /// </summary>
    [JsonProperty(PropertyName = "user_name")]
    public string UserName { get; protected set; }
    
    /// <summary>
    /// The UTC timestamp when the user started following the broadcaster.
    /// </summary>
    [JsonProperty(PropertyName = "followed_at")]
    public string FollowedAt { get; protected set;  }
}