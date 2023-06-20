using OWP.PreMatchNotification.Shared.Constants;
using OWP.PreMatchNotification.Shared.Models;
using System.Collections.Immutable;

namespace OWP.PreMatchNotification.Shared.Services;

public interface MessageFactory
{
    List<MessageModel> Transform(IList<ChangedValue> changedValues);
}

public class MessageFactoryImp : MessageFactory
{
    #region MessageTemplate

    private static readonly ImmutableList<MessageTemplate> messageTemplates = ImmutableList<MessageTemplate>.Empty.AddRange(new[]
    {
        new MessageTemplate
        {
             MessageType=(int)MsgTemplateCode.DayShiftAssignmentInsert,
             MessageContent="[DayShift] You have NEW ASSIGNMENT <b><bettype></b> in <b><League></b>",
             IsImportant = true
        },
        new ()
        {
             MessageType=(int)MsgTemplateCode.DayShiftAssignmentDelete,
             MessageContent="[DayShift] You have been REMOVED from <b><bettype></b> in <b><League></b>",
             IsImportant = true
        },
        new ()
        {
             MessageType=(int)MsgTemplateCode.DayShiftOtherAssignmentUpdate,
             MessageContent="[DayShift] <bettype>'s Assignee has been updated from <b><oldvalue></b> to <newvalue> in your assignment <b><League></b>",
             IsImportant = true
        },
        new ()
        {
             MessageType=(int)MsgTemplateCode.NightShiftAssignmentInsert,
             MessageContent="[NightShift] You have NEW ASSIGNMENT <b><bettype></b> in <b><League></b>",
             IsImportant = true
        },
        new ()
        {
             MessageType=(int)MsgTemplateCode.NightShiftAssignmentDelete,
             MessageContent="[NightShift] You have been REMOVED from <b><bettype></b> in <b><League></b>",
             IsImportant = true
        },
        new ()
        {
             MessageType=(int)MsgTemplateCode.NightShiftOtherAssignmentUpdate,
             MessageContent="[NightShift] <bettype>'s Assignee has been updated from <b><oldvalue></b> to <newvalue> in your assignment <b><League></b>",
             IsImportant = true
        },
        new ()
        {
            MessageType = (int)MsgTemplateCode.NewLeague,
            MessageContent = "New Add <League>",
            IsImportant = true,
            IsBlinking = true
        },
        new ()
        {
             MessageType=(int)MsgTemplateCode.LGAAddLiveMatch,
             MessageContent="ADD LIVE for <K.O Time> - <League> - <Home> -vs- <Away>",
             IsImportant = true,
             IsBlinking = true
        },
        new ()
        {
             MessageType=(int)MsgTemplateCode.LGACancelLiveMatch,
             MessageContent="CANCEL LIVE for <K.O Time> - <League> - <Home> -vs- <Away>",
             IsImportant = true,
             IsBlinking = true
        },
        new ()
        {
             MessageType=(int)MsgTemplateCode.LGAChangeMatchStatus,
             MessageContent="Has been <newvalue> for match: <League> - <Home> -vs- <Away>; Previous <oldvalue>",
             IsImportant = true,
             IsBlinking = true
        },
        new MessageTemplate
        {
             MessageType=(int)MsgTemplateCode.UpdateHome,
             MessageContent="<b>Home Team</b> Update from <League> - <oldvalue> -vs- <Away> to <League> - <newvalue> -vs- <Away>",
             IsImportant = true
        },
        new MessageTemplate
        {
             MessageType=(int)MsgTemplateCode.UpdateAway,
             MessageContent="<b>Away Team</b> Update from <League> - <Home> -vs- <oldvalue> to <League> - <Home> -vs- <newvalue>",
             IsImportant = true
        },
        new MessageTemplate
        {
             MessageType=(int)MsgTemplateCode.UpdateKOTime,
             MessageContent="<b>K.O time</b> Update from <oldvalue> to <newvalue> for <League> - <Home> -vs- <Away>",
             IsImportant = true
        }
    });

    #endregion MessageTemplate

    #region MessageModelRaw

    private class MessageModelRaw
    {
        public string MessageContent { get; set; }

        public bool IsImportant { get; set; }

        public int MessageType { get; set; }

        public bool IsBlinking { get; set; }

        public List<int> Assignees { get; set; }
    }

    #endregion MessageModelRaw

    private static readonly Dictionary<int, MessageTemplate> messageTemplateMap = messageTemplates.ToDictionary(m => m.MessageType, m => m);

    public List<MessageModel> Transform(IList<ChangedValue> changedValues)
    {
        var messageModels = TransformChangedValueToMessageModel(changedValues);
        return Reduce(messageModels);
    }

    private static IEnumerable<MessageModelRaw> TransformChangedValueToMessageModel(IEnumerable<ChangedValue> changedValues)
    {
        return changedValues
            .Select(g =>
            {
                var messageTemplate = messageTemplateMap[g.MessageTypeId];
                var content = CreateMessageContent(g);

                return new MessageModelRaw
                {
                    IsImportant = messageTemplate.IsImportant,
                    MessageContent = content,
                    MessageType = g.MessageTypeId,
                    Assignees = g.ReceivedUserIds,
                    IsBlinking = messageTemplate.IsBlinking
                };
            }).ToList();
    }

    private static string CreateMessageContent(ChangedValue value)
    {
        var messageTemplate = messageTemplateMap[value.MessageTypeId];
        var content = messageTemplate.MessageContent;

        content = content.Replace("<newvalue>", "<b><newvalue></b>");

        if (value.MessageTypeId is (int)MsgTemplateCode.UpdateHome or (int)MsgTemplateCode.UpdateAway or (int)MsgTemplateCode.UpdateKOTime)
        {
            content = content.Replace("<oldvalue>", "<b><oldvalue></b>");
        }

        content = content.Replace("<newvalue>", string.IsNullOrWhiteSpace(value.NewValue) ? "[]" : value.NewValue);
        content = content.Replace("<oldvalue>", string.IsNullOrWhiteSpace(value.OldValue) ? "[]" : value.OldValue);
        content = content.Replace("<League>", value.League);
        content = content.Replace("<Home>", value.Home);
        content = content.Replace("<Away>", value.Away);
        content = content.Replace("<User>", string.Empty);
        content = content.Replace("<bettype>", value.BetTypeName);
        content = content.Replace("<K.O Time>", value.KoTime);

        return content;
    }

    private static List<MessageModel> Reduce(IEnumerable<MessageModelRaw> messageModels)
    {
        return messageModels.GroupBy(m => new { m.MessageContent, m.IsImportant, m.MessageType, m.IsBlinking })
            .Select(m =>
            {
                var assignees = m.SelectMany(x => x.Assignees).Distinct().ToArray();
                return new MessageModel
                {
                    MessageType = m.Key.MessageType,
                    MessageContent = m.Key.MessageContent,
                    IsImportant = m.Key.IsImportant,
                    ReceivedUserIds = assignees.Select(a => a.ToString()).ToList(),
                    IsBlinking = m.Key.IsBlinking
                };
            })
            .ToList();
    }
}