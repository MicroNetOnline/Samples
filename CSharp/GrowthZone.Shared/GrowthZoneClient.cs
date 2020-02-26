using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace GrowthZone.Shared
{
    public sealed class GrowthZoneClient : IDisposable
    {
        public const string Host = "";
        public const string ClientId = "";
        public const string ClientSecret = "";
        
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
