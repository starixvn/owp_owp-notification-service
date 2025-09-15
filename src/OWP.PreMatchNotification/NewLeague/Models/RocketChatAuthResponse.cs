namespace OWP.PreMatchNotification.NewLeague.Models
{
    public class RocketChatAuthResponse
    {
        public string Status { get; set; }
        public AuthData Data { get; set; }

        public class AuthData
        {
            public string AuthToken { get; set; }
            public string UserId { get; set; }
        }
    }
}
