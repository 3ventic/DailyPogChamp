using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DailyPogChamp
{
    public class DiscordWebHookMessage
    {
        private static readonly HttpClientHandler HttpHandler = new HttpClientHandler()
        {
            AllowAutoRedirect = true,
            MaxAutomaticRedirections = 10,
            MaxConnectionsPerServer = 500,
            UseCookies = false,
            UseProxy = false
        };

        private readonly string _pogchamp;
        private string Payload => "{\"content\":\"https://static-cdn.jtvnw.net/emoticons/v2/" + _pogchamp + "/default/dark/3.0\"}";

        public DiscordWebHookMessage(string pogchamp)
        {
            _pogchamp = pogchamp;
        }

        public Task<HttpResponseMessage> Execute()
        {
            using var httpRequest = new HttpClient(HttpHandler, false);
            try
            {
                httpRequest.DefaultRequestHeaders.Add("Content-Type", "application/json");
                using var postData = new StringContent(Payload, Encoding.UTF8);
                return httpRequest.PostAsync(Environment.GetEnvironmentVariable("HOOK"), postData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error with hook: {ex}");
            }

            return (Task<HttpResponseMessage>) Task.CompletedTask;
        }
    }
}
