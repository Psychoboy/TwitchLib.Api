﻿using Newtonsoft.Json;

namespace TwitchLib.Api.Helix.Models.Moderation.ShieldModeStatus.UpdateShieldModeStatus;

/// <summary>
/// Update shield mode status response object.
/// </summary>
public class UpdateShieldModeStatusResponse
{
    /// <summary>
    /// A list that contains a single object with the broadcaster’s updated Shield Mode status.
    /// </summary>
    [JsonProperty(PropertyName = "data")]
    public ShieldModeStatus[] Data { get; protected set; }
}