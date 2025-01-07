﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitchLib.Api.Core;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Core.Exceptions;
using TwitchLib.Api.Core.Interfaces;
using TwitchLib.Api.Helix.Models.Channels.GetAdSchedule;
using TwitchLib.Api.Helix.Models.Channels.GetChannelEditors;
using TwitchLib.Api.Helix.Models.Channels.GetChannelFollowers;
using TwitchLib.Api.Helix.Models.Channels.GetChannelInformation;
using TwitchLib.Api.Helix.Models.Channels.GetChannelVIPs;
using TwitchLib.Api.Helix.Models.Channels.GetFollowedChannels;
using TwitchLib.Api.Helix.Models.Channels.ModifyChannelInformation;
using TwitchLib.Api.Helix.Models.Channels.SnoozeNextAd;
using TwitchLib.Api.Helix.Models.Channels.StartCommercial;

namespace TwitchLib.Api.Helix
{
    /// <summary>
    /// Channel related APIs
    /// </summary>
    public class Channels : ApiBase
    {
        public Channels(IApiSettings settings, IRateLimiter rateLimiter, IHttpCallHandler http) : base(settings, rateLimiter, http)
        {
        }

        #region GetChannelInformation

        /// <summary>
        /// Gets channel information for given user.
        /// </summary>
        /// <param name="broadcasterIds">list of user ids whose channel to get (max 100).</param>
        /// <param name="accessToken">optional access token to override the use of the stored one in the TwitchAPI instance</param>
        /// <returns cref="GetChannelInformationResponse"></returns>
        /// <exception cref="BadParameterException"></exception>
        public Task<GetChannelInformationResponse> GetChannelInformationAsync(List<string> broadcasterIds, string accessToken = null)
        {
            if (broadcasterIds.Count == 0 || broadcasterIds.Count > 100)
                throw new BadParameterException("boardcasterIds must contain between 1 and 100 broadcasterIds.");

            var getParams = broadcasterIds.Select(broadcasterId => new KeyValuePair<string, string>("broadcaster_id", broadcasterId)).ToList();

            return TwitchGetGenericAsync<GetChannelInformationResponse>("/channels", ApiVersion.Helix, getParams, accessToken);
        }

        /// <summary>
        /// Gets channel information for given user.
        /// backwards compatible, queries just one
        /// </summary>
        /// <param name="broadcasterId">The ID of the broadcaster whose channel you want to get.</param>
        /// <param name="accessToken">optional access token to override the use of the stored one in the TwitchAPI instance</param>
        /// <returns cref="GetChannelInformationResponse"></returns>
        /// <exception cref="BadParameterException"></exception>
        public Task<GetChannelInformationResponse> GetChannelInformationAsync(string broadcasterId, string accessToken = null) => GetChannelInformationAsync(new List<string>(){broadcasterId}, accessToken);
        #endregion

        #region ModifyChannelInformation

        /// <summary>
        /// Modifies channel information for given user.
        /// <para>Required scope: channel:manage:broadcast</para>
        /// </summary>
        /// <param name="broadcasterId">ID of the channel to be updated</param>
        /// <param name="request" cref="ModifyChannelInformationRequest"></param>
        /// <param name="accessToken">optional access token to override the use of the stored one in the TwitchAPI instance</param>
        /// <returns></returns>
        /// <exception cref="BadParameterException"></exception>
        public async Task<bool> ModifyChannelInformationAsync(string broadcasterId, ModifyChannelInformationRequest request, string accessToken = null)
        {
            if (string.IsNullOrEmpty(broadcasterId))
                throw new BadParameterException("broadcasterId must be set");

            var getParams = new List<KeyValuePair<string, string>>
            {
                new("broadcaster_id", broadcasterId)
            };

            var response = await TwitchPatchAsync("/channels", ApiVersion.Helix, JsonConvert.SerializeObject(request), getParams, accessToken);

            // Successfully updated the channel's properties if return code is 204 (No Content)
            return response.Key == 204;
        }
        #endregion

        #region GetChannelEditors

        /// <summary>
        /// Gets a list of users who have editor permissions for a specific channel.
        /// <para>Required scope: channel:read:editors</para>
        /// </summary>
        /// <param name="broadcasterId">The ID of the broadcaster whose editors you want to get.</param>
        /// <param name="accessToken">optional access token to override the use of the stored one in the TwitchAPI instance</param>
        /// <returns cref="GetChannelEditorsResponse"></returns>
        /// <exception cref="BadParameterException"></exception>
        public Task<GetChannelEditorsResponse> GetChannelEditorsAsync(string broadcasterId, string accessToken = null)
        {
            if (string.IsNullOrEmpty(broadcasterId))
                throw new BadParameterException("broadcasterId must be set");

            var getParams = new List<KeyValuePair<string, string>>
            {
                new("broadcaster_id", broadcasterId)
            };

            return TwitchGetGenericAsync<GetChannelEditorsResponse>("/channels/editors", ApiVersion.Helix, getParams, accessToken);
        }
        #endregion

        #region GetChannelVIPs

        /// <summary>
        /// Gets a list of the channel’s VIPs.
        /// Requires a user access token that includes the channel:read:vips scope.
        /// </summary>
        /// <param name="broadcasterId">The ID of the broadcaster whose list of VIPs you want to get.</param>
        /// <param name="userIds">Filters the list for specific VIPs. </param>
        /// <param name="first">The maximum number of items to return per page in the response. Max 100</param>
        /// <param name="after">The cursor used to get the next page of results.</param>
        /// <param name="accessToken">optional access token to override the use of the stored one in the TwitchAPI instance</param>
        /// <returns cref="GetChannelVIPsResponse"></returns>
        public Task<GetChannelVIPsResponse> GetVIPsAsync(string broadcasterId, List<string> userIds = null, int first = 20, string after = null, string accessToken = null)
        {
            if (string.IsNullOrEmpty(broadcasterId))
                throw new BadParameterException("broadcasterId must be set");

            if (first > 100 & first <= 0)
                throw new BadParameterException("first must be greater than 0 and less then 101");

            var getParams = new List<KeyValuePair<string, string>>
            {
                new("broadcaster_id", broadcasterId),
                new("first", first.ToString())
            };

            if (userIds != null)
            {
                if (userIds.Count == 0)
                    throw new BadParameterException("userIds must contain at least 1 userId if a list is included in the call");

                getParams.AddRange(userIds.Select(userId => new KeyValuePair<string, string>("userId", userId)));
            }

            if (!string.IsNullOrWhiteSpace(after))
                getParams.Add(new KeyValuePair<string, string>("after", after));

            return TwitchGetGenericAsync<GetChannelVIPsResponse>("/channels/vips", ApiVersion.Helix, getParams, accessToken);
        }

        #endregion

        #region AddChannelVIP

        /// <summary>
        /// Adds a VIP to the broadcaster’s chat room.
        /// Rate Limits: The channel may add a maximum of 10 VIPs within a 10 seconds period.
        /// Requires a user access token that includes the channel:manage:vips scope.
        /// </summary>
        /// <param name="broadcasterId">The ID of the broadcaster that’s granting VIP status to the user.</param>
        /// <param name="userId">The ID of the user to add as a VIP in the broadcaster’s chat room.</param>
        /// <param name="accessToken">optional access token to override the use of the stored one in the TwitchAPI instance</param>
        public Task AddChannelVIPAsync(string broadcasterId, string userId, string accessToken = null)
        {
            if (string.IsNullOrEmpty(broadcasterId))
                throw new BadParameterException("broadcasterId must be set");

            if (string.IsNullOrEmpty(userId))
                throw new BadParameterException("userId must be set");

            var getParams = new List<KeyValuePair<string, string>>
            {
                new("broadcaster_id", broadcasterId),
                new("user_id", userId),
            };

            return TwitchPostAsync("/channels/vips", ApiVersion.Helix, null, getParams, accessToken);
        }

        #endregion

        #region RemoveChannelVIP

        /// <summary>
        /// Removes a VIP from the broadcaster’s chat room.
        /// Rate Limits: The channel may remove a maximum of 10 VIPs within a 10 seconds period.
        /// Requires a user access token that includes the channel:manage:vips scope.
        /// </summary>
        /// <param name="broadcasterId">The ID of the user to remove as a VIP from the broadcaster’s chat room.</param>
        /// <param name="userId">The ID of the broadcaster that’s removing VIP status from the user.</param>
        /// <param name="accessToken">optional access token to override the use of the stored one in the TwitchAPI instance</param>
        public Task RemoveChannelVIPAsync(string broadcasterId, string userId, string accessToken = null)
        {
            if (string.IsNullOrEmpty(broadcasterId))
                throw new BadParameterException("broadcasterId must be set");

            if (string.IsNullOrEmpty(userId))
                throw new BadParameterException("userId must be set");

            var getParams = new List<KeyValuePair<string, string>>
            {
                new("broadcaster_id", broadcasterId),
                new("user_id", userId),
            };

            return TwitchDeleteAsync("/channels/vips", ApiVersion.Helix, getParams, accessToken);
        }

        #endregion
        
        #region GetFollowedChannels

        /// <summary>
        /// Gets a list of broadcasters that the specified user follows.
        /// <para>You can also use this endpoint to see whether a user follows a specific broadcaster.</para>
        /// </summary>
        /// <param name="userId">
        /// A user’s ID. Returns the list of broadcasters that this user follows.
        /// <para>This ID must match the user ID in the user OAuth token.</para>
        /// </param>
        /// <param name="broadcasterId">
        /// A broadcaster’s ID. Use this parameter to see whether the user follows this broadcaster.
        /// <para>If specified, the response contains this broadcaster if the user follows them.</para>
        /// <para>If not specified, the response contains all broadcasters that the user follows.</para>
        /// </param>
        /// <param name="first"> The maximum number of items to return per page in the response. Min: 1, Max: 100, Default: 20.</param>
        /// <param name="after">The cursor used to get the next page of results</param>
        /// <param name="accessToken"> Optional access token to override the use of the stored one in the TwitchAPI instance</param>
        /// <returns cref="GetFollowedChannelsResponse"></returns>
        public Task<GetFollowedChannelsResponse> GetFollowedChannelsAsync(string userId, string broadcasterId = null, int first = 20, string after = null, string accessToken = null)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new BadParameterException("userId must be set");
            
            if (first < 1 || first > 100)
                throw new BadParameterException("first cannot be less than 1 or greater than 100");

            var getParams = new List<KeyValuePair<string, string>>
            {
                new("user_id", userId)
            };
            
            if (!string.IsNullOrWhiteSpace(broadcasterId))
                getParams.Add(new KeyValuePair<string, string>("broadcaster_id", broadcasterId));
            if (first != 20)
                getParams.Add(new KeyValuePair<string, string>("first", first.ToString()));
            if (!string.IsNullOrWhiteSpace(after))
                getParams.Add(new KeyValuePair<string, string>("after", after));
            
            return TwitchGetGenericAsync<GetFollowedChannelsResponse>("/channels/followed", ApiVersion.Helix, getParams, accessToken);
        }

        #endregion

        #region GetChannelFollowers

        /// <summary>
        /// Gets a list of users that follow the specified broadcaster. Requires moderator:read:followers scope.
        /// <para>You can also use this endpoint to see whether a specific user follows the broadcaster.</para>
        /// </summary>
        /// <param name="broadcasterId">The broadcaster’s ID. Returns the list of users that follow this broadcaster.</param>
        /// <param name="userId">
        /// A users’s ID. Use this parameter to see whether the user follows this broadcaster.
        /// <para>If specified, the response contains this user if they follow the broadcaster.</para>
        /// <para>If not specified, the response contains all users follow the broadcaster</para>
        /// </param>
        /// <param name="first"> The maximum number of items to return per page in the response. Min: 1, Max: 100, Default: 20.</param>
        /// <param name="after">The cursor used to get the next page of results</param>
        /// <param name="accessToken"> Optional access token to override the use of the stored one in the TwitchAPI instance</param>
        /// <returns cref="GetFollowedChannelsResponse"></returns>
        public Task<GetChannelFollowersResponse> GetChannelFollowersAsync(string broadcasterId, string userId = null, int first = 20, string after = null, string accessToken = null)
        {
            if (string.IsNullOrWhiteSpace(broadcasterId))
                throw new BadParameterException("broadcasterId must be set");
            
            if (first < 1 || first > 100)
                throw new BadParameterException("first cannot be less than 1 or greater than 100");

            var getParams = new List<KeyValuePair<string, string>>
            {
                new("broadcaster_id", broadcasterId)
            };
            
            if (!string.IsNullOrWhiteSpace(userId))
                getParams.Add(new KeyValuePair<string, string>("user_id", userId));
            if (first != 20)
                getParams.Add(new KeyValuePair<string, string>("first", first.ToString()));
            if (!string.IsNullOrWhiteSpace(after))
                getParams.Add(new KeyValuePair<string, string>("after", after));
            
            return TwitchGetGenericAsync<GetChannelFollowersResponse>("/channels/followers", ApiVersion.Helix, getParams, accessToken);
        }

        #endregion

        #region GetAdSchedule

        /// <summary>
        /// Returns ad schedule related information, including snooze, when the last ad was run, when the next ad is scheduled, and if the channel is currently in pre-roll free time.
        /// </summary>
        /// <param name="broadcasterId">The broadcaster's ID. Ad schedule is relevant to this broadcaster, and so should the auth.</param>
        /// <param name="accessToken"> Optional access token to override the use of the stored one in the TwitchAPI instance</param>
        /// <returns cref="GetAdScheduleResponse"></returns>
        public Task<GetAdScheduleResponse> GetAdScheduleAsync(string broadcasterId, string accessToken = null)
        {
            if (string.IsNullOrWhiteSpace(broadcasterId))
                throw new BadParameterException("broadcasterId must be set");
            var getParams = new List<KeyValuePair<string, string>>
            {

                new("broadcaster_id", broadcasterId)
            };

            return TwitchGetGenericAsync<GetAdScheduleResponse>("/channels/ads", ApiVersion.Helix, getParams, accessToken);
        }

        #endregion

        #region SnoozeNextAd

        /// <summary>
        /// If available, pushes back the timestamp of the upcoming automatic mid-roll ad by 5 minutes. This endpoint duplicates the snooze functionality in the creator dashboard’s Ads Manager.
        /// </summary>
        /// <param name="broadcasterId">The broadcaster's ID. Ad snoozing is relevant to this broadcaster, and so should the auth.</param>
        /// <param name="accessToken"> Optional access token to override the use of the stored one in the TwitchAPI instance</param>
        /// <returns cref="SnoozeNextAdResponse"></returns>
        public Task<SnoozeNextAdResponse> SnoozeNextAdAsync(string broadcasterId, string accessToken = null)
        {
            if (string.IsNullOrWhiteSpace(broadcasterId))
                throw new BadParameterException("broadcasterId must be set");
            var getParams = new List<KeyValuePair<string, string>>
            {
                new("broadcaster_id", broadcasterId)
            };

            return TwitchPostGenericAsync<SnoozeNextAdResponse>("/channels/ads/schedule/snooze", ApiVersion.Helix, null, getParams, accessToken);
        }

        #endregion
        
        #region StartCommercial

        /// <summary>
        /// <para><see href="https://dev.twitch.tv/docs/api/reference/#start-commercial">
        /// Twitch Docs: Start Commercial</see></para>
        /// <para>Starts a commercial on the specified channel.</para>
        /// <para>Only partners and affiliates may run commercials and they must be streaming live at the time.
        /// Only the broadcaster may start a commercial - the broadcaster’s editors and moderators may not start commercials on behalf of the broadcaster.</para>
        /// <para><b>Requires a user access token that includes the channel:edit:commercial scope.</b></para>
        /// </summary>
        /// <param name="request" cref="StartCommercialRequest"></param>
        /// <param name="accessToken">Optional access token to override the use of the stored one in the TwitchAPI instance.</param>
        /// <returns cref="StartCommercialResponse"></returns>
        public Task<StartCommercialResponse> StartCommercialAsync(StartCommercialRequest request, string accessToken = null)
        {
            return TwitchPostGenericAsync<StartCommercialResponse>("/channels/commercial", ApiVersion.Helix, JsonConvert.SerializeObject(request), null, accessToken);
        }

        #endregion
    }
}
