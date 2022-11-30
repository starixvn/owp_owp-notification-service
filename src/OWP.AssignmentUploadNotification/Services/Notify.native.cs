/* namespace OWP.AssignmentUploadNotification.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Models;
    using OWP.Models;
    using OWP.Services;
    using Utils;

    public interface Notify
    {
        void NotifyMessagesToUser(List<MessageModel> messages);
    }

    public class NotifyImp : Notify
    {
        public void NotifyMessagesToUser(List<MessageModel> messages)
        {
            var userMessages = MapMessagesToUserName(messages);

            SendMessageToUser(userMessages);
        }

        private static Dictionary<string, List<Message>> MapMessagesToUserName(List<MessageModel> messages)
        {
            var userMessages = new Dictionary<string, List<Message>>();
            foreach (var message in messages)
            {
                var userNames = message.ReceivedUserNameList.Split(',').ToList();
                MapMessageToUserName(userMessages, message, userNames);
            }

            return userMessages;
        }

        private static void SendMessageToUser(Dictionary<string, List<Message>> userMessages)
        {
            foreach (var user in userMessages)
            {
                NotificationEngine.Singleton.Notify(new NotificationEventArgs
                {
                    UserId = user.Key,
                    Messages = user.Value
                });
            }
        }

        private static void MapMessageToUserName(Dictionary<string, List<Message>> userMessages, MessageModel message, List<string> userNames)
        {
            foreach (var userName in userNames)
            {
                var newMessage = new Message
                {
                    MessageContent = message.MessageContent,
                    MessageType = message.MessageType,
                    IsImportant = message.IsImportant
                };
                var userMessage = userMessages.GetValue(userName, new List<Message>());

                if (userMessage.Count == 0)
                {
                    userMessages.Add(userName, new List<Message> { newMessage });
                }
                else
                {
                    userMessage.Add(newMessage);
                }
            }
        }
    }
} */