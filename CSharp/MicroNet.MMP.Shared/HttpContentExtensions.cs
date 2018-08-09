using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MicroNet.MMP.Shared
{
    public static class HttpContentExtensions
    {
        /// <summary>
        /// Reads the stream as a JSON object.
        /// </summary>
        /// <typeparam name="T">The type of the instance to deserialize to.</typeparam>
        /// <param name="httpContent">The HTTP content to read.</param>
        /// <returns>The item that was read from the stream.</returns>
        public static async Task<T> ReadAsJsonAsync<T>(this HttpContent httpContent)
        {
            if (httpContent == null)
            {
                throw new ArgumentNullException(nameof(httpContent));
            }

            return JsonConvert.DeserializeObject<T>(await httpContent.ReadAsStringAsync());
        }
    }
}