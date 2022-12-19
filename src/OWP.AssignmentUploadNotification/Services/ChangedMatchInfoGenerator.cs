namespace OWP.AssignmentUploadNotification.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Constants;
    using Models;

    public interface ChangedMatchInfoGenerator
    {
        List<ChangedValue> Create(GeneralMatch oldMatch, GeneralMatch newMatch, List<Assignee> existingAssignees);
    }

    public class ChangedMatchInfoGeneratorImp : ChangedMatchInfoGenerator
    {
        private const string EVENT_VS = " -vs- ";

        private static readonly List<string> MatchInfoPropertyNames = new List<string>
        {
            "MatchNo",
            "OtherTime",
            "KickOffTime",
            "Agent",
            "Channel",
            "Division",
            "MatchOffer",
            "NoOfHDP",
            "Phone",
            "Provider",
            "OtherProvider",
            "Remark",
            "Round",
            "TV",
            "WB",
            "Website1",
            "Website2",
            "Website3",
            "Website4",
            "Home"
        };

        public List<ChangedValue> Create(GeneralMatch oldMatch, GeneralMatch newMatch, List<Assignee> existingAssignees)
        {
            var ignored = IsIgnoredMinorMatch(newMatch.MatchType);
            if (ignored)
            {
                return new List<ChangedValue>();
            }

            var changedValueList = new List<ChangedValue>();
            var changedList = GetListValueChanged(oldMatch, newMatch);
            foreach (var changed in changedList)
            {
                var changedValues = existingAssignees.Select(a => new ChangedValue
                {
                    MessageTypeId = changed.MessageTypeId,
                    NewValue = GetNewValue(newMatch, changed),
                    OldValue = GetOldValue(oldMatch, changed),
                    League = newMatch.LeagueName,
                    MatchNo = newMatch.MatchNo,
                    EventName = newMatch.Home + EVENT_VS + newMatch.Away,
                    AssigneeId = a.AssigneeId,
                    AssigneeName = a.AssigneeName
                });

                changedValueList.AddRange(changedValues);
            }

            return changedValueList;
        }

        private static string GetNewValue(GeneralMatch newMatch, ChangedValue changed)
        {
            return changed.MessageTypeId == MessageType.GetMessageTypeIdForChangedMatchInfo("Home")
                ? changed.NewValue + EVENT_VS + newMatch.Away
                : changed.NewValue;
        }

        private static string GetOldValue(GeneralMatch newMatch, ChangedValue changed)
        {
            return changed.MessageTypeId == MessageType.GetMessageTypeIdForChangedMatchInfo("Home")
                ? changed.OldValue + EVENT_VS + newMatch.Away
                : changed.OldValue;
        }

        public List<ChangedValue> GetListValueChanged(GeneralMatch oldMatch, GeneralMatch newMatch)
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

            return changedValues.Where(a => a.MessageTypeId != 0).ToList();
        }

        private static PropertyInfo GetProperty(GeneralMatch match, string propertyName)
        {
            return match.GetType().GetProperties().Single(p => p.Name == propertyName);
        }

        public static bool IsIgnoredMinorMatch(int matchType)
        {
            var minorGenerateList = new List<int> {
                (int)MinorBettype.Specific15Mins,
                (int)MinorBettype.Specific10Mins,
                (int)MinorBettype.FastMarket,
                (int)MinorBettype.AllInOne,
                (int)MinorBettype.Specific15MinsCorners,
                (int)MinorBettype.Specific15MinsTotalBookings
            };

            return minorGenerateList.Contains(matchType);
        }
    }
}
