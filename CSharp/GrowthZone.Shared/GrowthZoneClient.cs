﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace GrowthZone.Shared
{
    public sealed class GrowthZoneClient : IDisposable
    {
        //public const string Host = "https://growthzone.growthzonedev.com";
        //public const string ClientId = "<client id>";
        //public const string ClientSecret = "<client secret>";

        public const string Host = "http://localtest.me:12221";
        public const string ClientId = "73WLJTHDailzO1s22deT";
        public const string ClientSecret = "lG94S1LyeKLpvZzok2fTseDkobanBNdhgRKA322R";

        //public const string Host = "https://growthzone.growthzoneapp.com";
        //public const string ClientId = "a";
        //public const string ClientSecret = "b";

        readonly HttpClient _httpClient;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="endpoint">The API endpoint to connect to.</param>
        /// <param name="accessToken">The access token for the login.</param>
        public GrowthZoneClient(Uri endpoint, string accessToken)
        {
            _httpClient = new HttpClient { BaseAddress = endpoint };

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _httpClient.Dispose();
        }

        /// <summary>
        /// Gets the currently authenticated user.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The current authenticated user.</returns>
        public async Task<ClaimsResponse> GetClaimsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var httpResponse = await _httpClient.GetAsync("api/contacts/root/claims", cancellationToken);
            httpResponse.EnsureSuccessStatusCode();

            return await httpResponse.Content.ReadAsJsonAsync<ClaimsResponse>();
        }
    }
}