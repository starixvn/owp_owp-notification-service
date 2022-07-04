using OWP.OtherSportNotification.Constants;
using OWP.OtherSportNotification.Helpers;
using OWP.OtherSportNotification.Models;
using System.Reflection;

namespace OWP.OtherSportNotification.Services;

public interface ChangedMatchInfoGenerator
{
    List<ChangedValue> Create(MatchInfo oldMatch, MatchInfo newMatch, List<Assignee> existingAssignees);
}

public class ChangedMatchInfoGeneratorImp : ChangedMatchInfoGenerator
{
    private const string EVENT_VS = " -vs- ";

    private static readonly List<string> MatchInfoPropertyNames = new List<string>
    {
        "MatchNo",
        "Category",
        "MatchOffer",
        "NoHDP",
        "AutoLink",
        "OtherProvider",
        "IPTV",
        "WB",
        "Remark",
        "Reporter",
        "Provider",
        "IsNeutral"
    };

    public List<ChangedValue> Create(MatchInfo oldMatch, MatchInfo newMatch, List<Assignee> existingAssignees)
    {
        var changedValueList = new List<ChangedValue>();
        var changedList = GetListValueChanged(oldMatch, newMatch);
        foreach (var changed in changedList)
        {
            var changedValues = existingAssignees.Select(a => new ChangedValue
            {
                MessageTypeId = changed.MessageTypeId,
                NewValue = GetNewValue(newMatch, changed),
                OldValue = GetOldValue(oldMatch, changed),
                League = newMatch.League,
                MatchNo = $"{newMatch.MatchNo}",
                EventName = UpdatedValueHelper.CreateEventName(newMatch),
                UserId = a.AssigneeId,
                Username = a.AssigneeName
            });

            changedValueList.AddRange(changedValues);
        }

        return changedValueList;
    }

    public static string GetNewValue(MatchInfo newMatch, ChangedValue changed)
    {
        if (changed.MessageTypeId == MessageType.GetMessageTypeIdForChangedMatchInfo("Home"))
        {
            return changed.NewValue + EVENT_VS + newMatch.Away;
        }

        if (changed.MessageTypeId == MessageType.GetMessageTypeIdForChangedMatchInfo("IsNeutral"))
        {
            return (changed.NewValue == "False" ? newMatch.Home : (newMatch.Home + " (N)")) + EVENT_VS + newMatch.Away;
        }

        return changed.NewValue;
    }

    public static string GetOldValue(MatchInfo newMatch, ChangedValue changed)
    {
        if (changed.MessageTypeId == MessageType.GetMessageTypeIdForChangedMatchInfo("Home"))
        {
            return changed.OldValue + EVENT_VS + newMatch.Away;
        }

        if (changed.MessageTypeId == MessageType.GetMessageTypeIdForChangedMatchInfo("IsNeutral"))
        {
            return (changed.OldValue == "False" ? newMatch.Home : (newMatch.Home + " (N)")) + EVENT_VS + newMatch.Away;
        }

        return changed.OldValue;
    }

    public List<ChangedValue> GetListValueChanged(MatchInfo oldMatch, MatchInfo newMatch)
    {
        var changedValues = new List<ChangedValue>();
        foreach (var propertyName in MatchInfoPropertyNames)
        {
            var property = GetProperty(oldMatch, propertyName);
            var oldValue = property.GetValue(oldMatch)?.ToString() ?? string.Empty;
            var newValue = property.GetValue(newMatch)?.ToString() ?? string.Empty;
            if (oldValue.Equals(newValue))
            {
                continue;
            }

            var messageModel = new ChangedValue
            {
                OldValue = oldValue,
                NewValue = newValue,
                MessageTypeId = MessageType.GetMessageTypeIdForChangedMatchInfo(propertyName)
            };

            changedValues.Add(messageModel);
        }

        return changedValues.ToList();
    }

    public static PropertyInfo GetProperty(MatchInfo match, string propertyName)
    {
        return match.GetType().GetProperties().Single(p => p.Name == propertyName);
    }
}
