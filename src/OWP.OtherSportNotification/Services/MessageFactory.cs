
using OWP.OtherSportNotification.Constants;
using OWP.OtherSportNotification.Models;
using System.Collections.Immutable;
// ReSharper disable All

namespace OWP.OtherSportNotification.Services;

public interface MessageFactory
{
    List<MessageModel> Transform(List<ChangedValue> changedValues, string actor);
}

public class MessageFactoryImp : MessageFactory
{
    #region MessageTemplate

    private static readonly ImmutableList<MessageTemplate> messageTemplates = ImmutableList<MessageTemplate>.Empty.AddRange(new[]
    {
            new MessageTemplate
            {
                MessageType = (int)MsgTemplateCode.EditMatchNo,
                MessageContent = "<b>Match No.</b> Updated from <oldvalue> to <newvalue> for <Match.No> - <League> - <Event> updated by <User>"
            },
            new MessageTemplate
            {
                MessageType = (int)MsgTemplateCode.EditMatchStatus,
                MessageContent = "<b>Match Status</b> Updated from <oldvalue> to <newvalue> for <Match.No> - <League> - <Event> updated by <User>",
                IsImportant= true
            },
            new MessageTemplate
            {
                MessageType = (int)MsgTemplateCode.EditCategory,
                MessageContent = "<b>Category</b> Updated from <oldvalue> to <newvalue> for <Match.No> - <League> - <Event> updated by <User>"
            },
            new MessageTemplate
            {
                 MessageType=(int)MsgTemplateCode.EditNoOfHDP,
                 MessageContent="<b>No. HDP</b> Updated from <oldvalue> to <newvalue> for <Match.No> - <League> - <Event> updated by <User>"
            },
            new MessageTemplate
            {
                 MessageType=(int)MsgTemplateCode.EditMatchOffer,
                 MessageContent="<b>Match Offer</b> Updated from <oldvalue> to <newvalue> for <Match.No> - <League> - <Event> updated by <User>"
            },
            new MessageTemplate
            {
                 MessageType=(int)MsgTemplateCode.EditAutoLink,
                 MessageContent="<b>Auto Link</b> Updated from <oldvalue> to  <newvalue> for <Match.No> - <League> - <Event> updated by <User>"
            },
            new MessageTemplate
            {
                 MessageType=(int)MsgTemplateCode.EditProvider,
                 MessageContent="<b>Provider</b> Updated from <oldvalue> to <newvalue> for <Match.No> - <League> - <Event> updated by <User>"
            },
            new MessageTemplate
            {
                 MessageType=(int)MsgTemplateCode.EditOtherProvider,
                 MessageContent="<b>Other Provider</b> Updated from <oldvalue> to <newvalue> for <Match.No> - <League> - <Event> updated by <User>"
            },
             new MessageTemplate
            {
                 MessageType=(int)MsgTemplateCode.EditIPTV,
                 MessageContent="<b>IPTV</b> Updated from <oldvalue> to <newvalue> for <Match.No> - <League> - <Event> updated by <User>"
            },
            new MessageTemplate
            {
                 MessageType=(int)MsgTemplateCode.EditWB,
                 MessageContent="<b>Streaming</b> Info (WB) Updated from <oldvalue> to <newvalue> for <Match.No> - <League> - <Event> updated by <User>"
            },
            new MessageTemplate
            {
                 MessageType=(int)MsgTemplateCode.EditRemark,
                 MessageContent="<b>Remark</b> Updated from <oldvalue> to <newvalue> for <Match.No> - <League> - <Event> updated by <User>"
            },
            new MessageTemplate
            {
                 MessageType=(int)MsgTemplateCode.EditMaxbet,
                 MessageContent="<b>Max Bet</b> of <bettype> Updated from <oldvalue> to <newvalue> for <Match.No> - <League> - <Event> updated by <User>"
            },
            new MessageTemplate
            {
                 MessageType=(int)MsgTemplateCode.EditSpread,
                 MessageContent="<b>Spread</b> of <bettype> Updated from <oldvalue> to <newvalue> for <Match.No> - <League> - <Event> updated by <User>"
            },
            new MessageTemplate
            {
                 MessageType=(int)MsgTemplateCode.EditReporter,
                 MessageContent="<b>Reporter</b> Updated from <oldvalue> to <newvalue> for <Match.No> - <League> - <Event> updated by <User>"
            },
            new MessageTemplate
            {
                 MessageType=(int)MsgTemplateCode.AssignmentInsert,
                 MessageContent="You have NEW ASSIGNMENT <bettype> in <Match.No> - <League> - <Event> - <KOTime> updated by <User>",
                 IsImportant = true
            },
            new MessageTemplate
            {
                 MessageType=(int)MsgTemplateCode.OtherAssignmentInsert,
                 MessageContent="You have NEW ASSIGNMENT <bettype> in <Match.No> - <League> - <Event> - <KOTime> updated by <User>",
                 IsImportant = true
            },
            new MessageTemplate
            {
                 MessageType=(int)MsgTemplateCode.AssignmentUpdate,
                 MessageContent="UPDATED ASSIGNEE of <bettype> from <oldvalue> change to <newvalue> for <Match.No> - <League> - <Event> - <KOTime> Updated by <User> ",
                 IsImportant = true
            },
            new MessageTemplate
            {
                 MessageType=(int)MsgTemplateCode.OtherAssignmentUpdate,
                 MessageContent="UPDATED ASSIGNEE of <bettype> from <oldvalue> change to <newvalue> for <Match.No> - <League> - <Event> - <KOTime> Updated by <User> ",
                 IsImportant = true
            },
            new MessageTemplate
            {
                 MessageType=(int)MsgTemplateCode.AssignmentDelete,
                 MessageContent="You have been REMOVED from <bettype> in <Match.No> - <League> - <Event> - <KOTime> updated by <User>",
                 IsImportant = true
            },
            new MessageTemplate
            {
                 MessageType=(int)MsgTemplateCode.OtherAssignmentDelete,
                 MessageContent="You have been REMOVED from <bettype> in <Match.No> - <League> - <Event> - <KOTime> updated by <User>",
                 IsImportant = true
            },
            new MessageTemplate
            {
                 MessageType=(int)MsgTemplateCode.EditAssignmentNote,
                 MessageContent="<b>Your Assignment Note</b> Updated from <oldvalue> to <newvalue> for <Match.No> - <League> - <Event> updated by <User>"
            },
            new MessageTemplate
            {
                MessageType = (int) MsgTemplateCode.EditNeutral,
                MessageContent = "<Match.No> - <League> - <newvalue> updated by <User>, previous: [ <Match.No> - <League> - <oldvalue>]"
            }
        });

    #endregion MessageTemplate

    #region MessageGroup

    private class MessageGroup
    {
        public MessageGroup(ChangedValue value)
        {
            AssignmentNote = value.AssignmentNote;
            BetTypeName = value.BetTypeName;
            MessageTypeId = value.MessageTypeId;
            NewValue = value.NewValue;
            OldValue = value.OldValue;
            League = value.League;
            EventName = value.EventName;
            MatchNo = value.MatchNo;
            IsWatcher = value.IsWatcher;
            EventDateTime = value.EventDateTime;
        }

        public string AssignmentNote { get; set; }

        public string BetTypeName { get; set; }

        public int MessageTypeId { get; set; }

        public string NewValue { get; set; }

        public string OldValue { get; set; }

        public string League { get; set; }

        public string EventName { get; set; }

        public string MatchNo { get; set; }

        public bool IsWatcher { get; set; }

        public DateTime EventDateTime { get; set; }
    }

    #endregion MessageGroup

    #region MessageModelRaw

    private class MessageModelRaw
    {
        public string MessageContent { get; set; }

        public bool IsImportant { get; set; }

        public int MessageType { get; set; }

        public List<Assignee> Assignees { get; set; }
    }

    #endregion MessageModelRaw

    private static readonly Dictionary<int, MessageTemplate> messageTemplateMap = messageTemplates.ToDictionary(m => m.MessageType, m => m);

    public List<MessageModel> Transform(List<ChangedValue> changedValues, string actor)
    {
        var messageModels = TransformChangedValueToMessageModel(changedValues, actor);
        return Reduce(messageModels);
    }

    private static List<MessageModelRaw> TransformChangedValueToMessageModel(List<ChangedValue> changedValues, string actor)
    {
        return changedValues.GroupBy(v => new MessageGroup(v))
            .Select(g =>
            {
                var messageTemplate = messageTemplateMap[g.Key.MessageTypeId];
                var content = CreateMessageContent(g.Key, actor);

                return new MessageModelRaw
                {
                    IsImportant = messageTemplate.IsImportant,
                    MessageContent = content,
                    MessageType = g.Key.MessageTypeId,
                    Assignees = g.Select(i => new Assignee { AssigneeId = i.UserId, AssigneeName = i.Username }).ToList()
                };
            }).ToList();
    }

    private static string CreateMessageContent(MessageGroup messageGroup, string actor)
    {
        var messageTemplate = messageTemplateMap[messageGroup.MessageTypeId];
        var content = messageTemplate.MessageContent;

        content = content.Replace("<newvalue>", "<b><newvalue></b>");
        content = content.Replace("<bettype>", "<b><bettype></b>");

        content = content.Replace("<newvalue>", string.IsNullOrWhiteSpace(messageGroup.NewValue) ? "[]" : messageGroup.NewValue);
        content = content.Replace("<oldvalue>", string.IsNullOrWhiteSpace(messageGroup.OldValue) ? "[]" : messageGroup.OldValue);
        content = content.Replace("<Match.No>", !string.IsNullOrEmpty(messageGroup.MatchNo) ? $"#{messageGroup.MatchNo}" : string.Empty);
        content = content.Replace("<League>", messageGroup.League);
        content = content.Replace("<Event>", messageGroup.EventName);
        content = content.Replace("<User>", actor);
        content = content.Replace("<bettype>", messageGroup.BetTypeName);
        content = content.Replace("<KOTime>", messageGroup.EventDateTime.ToString("MM/dd/yyyy hh:mm tt"));

        return content;
    }

    private static List<MessageModel> Reduce(List<MessageModelRaw> messageModels)
    {
        return messageModels
            .Select(m =>
            {
                return new MessageModel
                {
                    MessageType = m.MessageType,
                    MessageContent = m.MessageContent,
                    IsImportant = m.IsImportant,
                    ReceivedUserIds = m.Assignees.Select(a => a.AssigneeId.ToString()).Distinct().ToList()
                };
            })
            .ToList();
    }
}