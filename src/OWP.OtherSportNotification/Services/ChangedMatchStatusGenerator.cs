using OWP.OtherSportNotification.Constants;
using OWP.OtherSportNotification.Helpers;
using OWP.OtherSportNotification.Models;
using System.Globalization;

namespace OWP.OtherSportNotification.Services;

public interface ChangedMatchStatusGenerator
{
    List<ChangedValue> Create(ChangedMatchSnapshot snapshot, List<Assignee> existingAssignees);
}

public class ChangedMatchStatusGeneratorImp : ChangedMatchStatusGenerator
{
    public List<ChangedValue> Create(ChangedMatchSnapshot snapshot, List<Assignee> existingAssignees)
    {
        var isMatchStatusChanged = !string.Equals(snapshot.OldMatch.OfferLive, snapshot.NewMatch.OfferLive, StringComparison.CurrentCultureIgnoreCase);
        if (!isMatchStatusChanged)
        {
            return [];
        }

        return existingAssignees.Select(a => new ChangedValue
        {
            MessageTypeId = (int)MsgTemplateCode.EditMatchStatus,
            OldValue = snapshot.OldMatch.OfferLive,
            NewValue = snapshot.NewMatch.OfferLive,
            MatchNo = snapshot.NewMatch.MatchNo.ToString(CultureInfo.InvariantCulture),
            AdminsiteMatchID = snapshot.NewMatch.AdminsiteMatchId,
            League = snapshot.NewMatch.League,
            EventName = UpdatedValueHelper.CreateEventName(snapshot.NewMatch),
            UserId = a.AssigneeId,
            Username = a.AssigneeName
        }).ToList();
    }
}
