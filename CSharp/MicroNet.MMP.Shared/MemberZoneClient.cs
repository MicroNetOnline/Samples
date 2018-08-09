using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace MicroNet.MMP.Shared
{
    public sealed class MemberZoneClient : IDisposable
    {
        public const string Host = "https://gz-10958.growthzonebranch.com";
        public const string ClientId = "g5Txkot0B7EZi984wFXP7ygeBIuGQg3CuWnGc6JRT0Q";
        public const string ClientSecret = "047jDzUIphXCJVUP9hwqgw";

        //public const string Host = "http://localtest.me:12221";
        //public const string ClientId = "123ABC";
        //public const string ClientSecret = "JxEF1YrH2XCBQHoV708Yw";

        readonly HttpClient _httpClient;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="endpoint">The API endpoint to connect to.</param>
        /// <param name="accessToken">The access token for the login.</param>
        public MemberZoneClient(Uri endpoint, string accessToken)
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