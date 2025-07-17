using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace OWP.RocketChat.Handler
{
    public interface NotificationHandler
    {
        public Task Handle();
    }

    public class NotificationHandlerImp(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration) : NotificationHandler
    {
        public async Task Handle()
        {
            var client = httpClientFactory.CreateClient();
            var chatUrl = configuration["ChatServer:Url"];
            client.BaseAddress = new Uri(chatUrl);

            //var client = httpClientFactory.CreateClient("OWPChatAdmin");
            throw new NotImplementedException();
        }

        public async Task<(string Token, string UserId)> LoginToRocketChatAsync(HttpClient client, string username,
            string password)
        {
            var loginPayload = new { user = username, password = password };

            var response = await client.PostAsJsonAsync("login", loginPayload);
            response.EnsureSuccessStatusCode();

            var authResult = await response.Content.ReadFromJsonAsync<RocketChatAuthResponse>();

            return (authResult.Data.AuthToken, authResult.Data.UserId);
        }
    }
}
