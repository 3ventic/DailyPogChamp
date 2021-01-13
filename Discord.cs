using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DailyPogChamp
{
    public class DiscordWebHookMessage
    {
        private static readonly HttpClientHandler HttpHandler = new()
        {
            AllowAutoRedirect = true,
            MaxAutomaticRedirections = 10,
            MaxConnectionsPerServer = 500,
            AutomaticDecompression = DecompressionMethods.All,
            UseCookies = false,
            UseProxy = false
        };

        private static readonly HttpClient Http = new(HttpHandler, false) { Timeout = TimeSpan.FromSeconds(10) };

        private readonly long _pogchamp;
        private string Payload => "{\"content\":\"https://static-cdn.jtvnw.net/emoticons/v2/" + _pogchamp + "/default/dark/3.0\"}";

        public DiscordWebHookMessage(long pogchamp)
        {
            _pogchamp = pogchamp;
        }

        public async Task<HttpResponseMessage?> ExecuteAsync()
        {
            Console.WriteLine("executing webhook");
            try
            {
                using var postData = new StringContent(Payload, Encoding.UTF8);
                postData.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                return await Http.PostAsync(Environment.GetEnvironmentVariable("HOOK"), postData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error with hook: {ex}");
            }

            return null;
        }
    }
}
