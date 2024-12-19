using OWP.PreMatchNotification.Assignment.Models;
using OWP.PreMatchNotification.Shared.Constants;
using OWP.PreMatchNotification.Shared.Models;

namespace OWP.PreMatchNotification.Assignment.Helper;

public static class AssignmentHelper
{
    public static int DetectMessageType(bool isDay)
        => isDay
            ? (int)MsgTemplateCode.DayShiftOtherAssignmentUpdate
            : (int)MsgTemplateCode.NightShiftOtherAssignmentUpdate;

    public static void DetectChangedValue(MyAssignmentDetectorModel model, List<ChangedValue> changedValues)
    {
        DetectChangedValueNewAssignment(model, changedValues);
        DetectChangedValueDeleteAssignment(model, changedValues);
    }

    public static void DetectChangedValueNewAssignment(MyAssignmentDetectorModel model, List<ChangedValue> changedValues)
    {
        if (!string.Equals(model.OldAssignment, model.NewAssignment) &&
            !string.IsNullOrWhiteSpace(model.NewAssignment))
        {
            changedValues.Add(new ChangedValue
            {
                League = model.LeagueName,
                BetTypeName = model.BettypeName,
                ReceivedUserIds = [model.UserIdNewAssign ?? 0],
                MessageTypeId = model.IsDay != null && model.IsDay.Value
                ? (int)MsgTemplateCode.DayShiftAssignmentInsert
                : (int)MsgTemplateCode.NightShiftAssignmentInsert
            });
        }
    }

    public static void DetectChangedValueDeleteAssignment(MyAssignmentDetectorModel model, List<ChangedValue> changedValues)
    {
        if (!string.Equals(model.OldAssignment, model.NewAssignment) &&
            !string.IsNullOrWhiteSpace(model.OldAssignment))
        {
            changedValues.Add(new ChangedValue
            {
                League = model.LeagueName,
                BetTypeName = model.BettypeName,
                ReceivedUserIds = [model.UserIdRemoved ?? 0],
                MessageTypeId = model.IsDay != null && model.IsDay.Value
                ? (int)MsgTemplateCode.DayShiftAssignmentDelete
                : (int)MsgTemplateCode.NightShiftAssignmentDelete
            });
        }
    }
}