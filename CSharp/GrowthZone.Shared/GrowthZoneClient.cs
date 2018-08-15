using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace GrowthZone.Shared
{
    public sealed class GrowthZoneClient : IDisposable
    {
        //public const string Host = "https://tardisenterprises.gz-10958.growthzonebranch.com";
        public const string Host = "https://growthzone.growthzonedev.com";
        public const string ClientId = "OG46DpRN3j0dnD3os9debN0djKXNllI6VH73jfaGRtI";
        public const string ClientSecret = "GeD13PT6MsdcHhD80kX0Vw";

        //public const string Host = "http://localtest.me:12221";
        //public const string ClientId = "123ABC";
        //public const string ClientSecret = "JxEF1YrH2XCBQHoV708Yw";

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