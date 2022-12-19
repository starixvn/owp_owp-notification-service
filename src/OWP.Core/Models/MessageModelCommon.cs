namespace OWP.Core.Models
{
    public class MessageModelCommon<T> where T : class
    {
        public T Message { get; set; }

        public IList<string> ReceivedUserIds{get; set; }
    }
}