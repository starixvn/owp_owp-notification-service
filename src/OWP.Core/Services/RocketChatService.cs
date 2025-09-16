using Microsoft.Extensions.Configuration;
using OWP.Core.Exceptions;
using System.Net.Http.Json;
using Fanex.Data.Repository;
using OWP.Core.Models;

namespace OWP.Core.Services
{
    public interface RocketChatService
    {
        Task LoginToRc(HttpClient client);

        HttpClient CreateClient();

        Task LogOutRC(HttpClient client);
    }

    public class RocketChatServiceImp(IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        IDynamicRepository repository) : RocketChatService
    {
        public async Task LoginToRc(HttpClient client)
        {
            try
            {
                var username = configuration["ChatServer:BotName"];
                var password = configuration["ChatServer:BotPassword"];
                var loginPayload = new { user = username, password };

                var response = await client.PostAsJsonAsync("api/v1/login", loginPayload);
                response.EnsureSuccessStatusCode();

                var authResult = await response.Content.ReadFromJsonAsync<RocketChatAuthResponse>();

                client.DefaultRequestHeaders.Add("X-Auth-Token", authResult.Data.AuthToken);
                client.DefaultRequestHeaders.Add("X-User-Id", authResult.Data.UserId);
            }
            catch (Exception exception)
            {
                throw new RocketChatException("Failed to login to Rocket.Chat", exception);
            }
        }

        public HttpClient CreateClient()
        {
            var chatUrl = configuration["ChatServer:Url"];
            var client = httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(chatUrl);
            client.Timeout = TimeSpan.FromMinutes(2);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            return client;
        }

        public async Task LogOutRC(HttpClient client)
        {
            await client.PostAsync("api/v1/logout", null);
        }

    }
}
