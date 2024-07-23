using Newtonsoft.Json;

namespace TwitchLib.Api.Helix.Models.EventSub.Conduits.Shards.UpdateConduitShards;

/// <summary>
/// Update conduit shards Response object.
/// </summary>
public class UpdateConduitShardsResponse
{
    /// <summary>
    /// <para>List of successful shard updates.</para>
    /// </summary>
    [JsonProperty(PropertyName = "data")]
    public Shard[] Shards { get; protected set; }

    /// <summary>
    /// <para>List of unsuccessful updates.</para>
    /// </summary>
    [JsonProperty(PropertyName = "errors")]
    public Error[] Errors { get; protected set; }
}