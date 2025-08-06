namespace OWP.Core.Exceptions
{
    public class RocketChatException(string message, Exception innerException) : Exception(message, innerException)
    {
    }
}
