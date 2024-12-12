﻿using Newtonsoft.Json;

namespace TwitchLib.Api.Helix.Models.Chat.Emotes;

/// <summary>
/// A emote set.
/// </summary>
public class EmoteSet : Emote
{
    /// <summary>
    /// The type of emote. 
    /// </summary>
    [JsonProperty("emote_type")]
    public string EmoteType { get; protected set; }

    /// <summary>
    /// An ID that identifies the emote set that the emote belongs to.
    /// </summary>
    [JsonProperty("emote_set_id")]
    public string EmoteSetId { get; protected set; }

    /// <summary>
    /// Contains the image URLs for the emote.
    /// </summary>
    [JsonProperty("images")]
    public EmoteImages Images { get; protected set; }

    /// <summary>
    /// The ID of the broadcaster who owns the emote.
    /// </summary>
    [JsonProperty("owner_id")]
    public string OwnerId { get; protected set; }
}