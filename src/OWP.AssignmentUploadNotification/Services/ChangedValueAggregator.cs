namespace OWP.AssignmentUploadNotification.Services;

using AutoMapper;
using Constants;
using Models;
using System.Collections.Generic;
using System.Linq;

public interface ChangedValueAggregator
{
    IEnumerable<ChangedValue> Aggregate(IEnumerable<ChangedValue> changedValues);
}

public class ChangedValueAggregatorImp(IMapper mapper) : ChangedValueAggregator
{
    internal class AssignmentGroup
    {
        public int MatchId { get; set; }

        public int BetTypeId { get; set; }

        public int AssigneeId { get; set; }

        public bool IsWatcher { get; set; }

        public bool IsIndirectChanged { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as AssignmentGroup ?? new AssignmentGroup();

            var condition1 = MatchId == other.MatchId && BetTypeId == other.BetTypeId && AssigneeId == other.AssigneeId;
            var condition2 = IsIndirectChanged == other.IsIndirectChanged && IsWatcher == other.IsWatcher;

            return condition1 && condition2;
        }

        public override int GetHashCode()
        {
            return MatchId * 17
                   + BetTypeId * 19
                   + AssigneeId * 29
                   + (IsWatcher ? 31 : 0)
                   + (IsIndirectChanged ? 37 : 0);
        }
    }

    public IEnumerable<ChangedValue> Aggregate(IEnumerable<ChangedValue> changedValues)
    {
        var changedValueList = changedValues.ToList();
        var nonAggregatedChangedValues = GetNonAggregatedChangedValues(changedValueList);
        var indirectMainAssignmentChangedValues = GetIndirectMainAssignmentChangedValues(changedValueList);
        var indirectOtherAssignmentChangedValues = GetIndirectOtherAssignmentChangedValues(changedValueList);

        var aggregatedValueForIndirectMainAssignments = GetAggregatedValueForIndirectMainAssignments(indirectMainAssignmentChangedValues);
        var aggregatedValueForIndirectOtherAssignments = GetAggregatedValueForIndirectOtherAssignments(indirectOtherAssignmentChangedValues);

        return nonAggregatedChangedValues.Union(aggregatedValueForIndirectMainAssignments)
            .Union(aggregatedValueForIndirectOtherAssignments);
    }

    public static IEnumerable<ChangedValue> GetNonAggregatedChangedValues(List<ChangedValue> changedValues)
    {
        return changedValues.Where(m =>
        {
            var isDirectChanged = !m.IsIndirectChangedAssignment && !m.IsWatcher;
            var isMainAssignment = m.MessageTypeId is (int)MsgTemplateCode.AssignmentDelete or (int)MsgTemplateCode.AssignmentInsert;
            var isOtherAssignment = m.MessageTypeId is (int)MsgTemplateCode.OtherAssignmentDelete or (int)MsgTemplateCode.OtherAssignmentInsert;

            return isDirectChanged || !isMainAssignment && !isOtherAssignment;
        });
    }

    public static IEnumerable<ChangedValue> GetIndirectOtherAssignmentChangedValues(List<ChangedValue> changedValues)
    {
        return changedValues.Where(m =>
        {
            var isIndirectChangedAssignment = m.IsIndirectChangedAssignment || m.IsWatcher;
            var isOtherAssignment = m.MessageTypeId is (int)MsgTemplateCode.OtherAssignmentDelete or (int)MsgTemplateCode.OtherAssignmentInsert;

            return isIndirectChangedAssignment && isOtherAssignment;
        });
    }

    public static IEnumerable<ChangedValue> GetIndirectMainAssignmentChangedValues(IEnumerable<ChangedValue> changedValues)
    {
        return changedValues.Where(m =>
        {
            var isIndirectChangedAssignment = m.IsIndirectChangedAssignment || m.IsWatcher;
            var isMainAssignment = m.MessageTypeId is (int)MsgTemplateCode.AssignmentDelete or (int)MsgTemplateCode.AssignmentInsert;

            return isIndirectChangedAssignment && isMainAssignment;
        });
    }

    public IEnumerable<ChangedValue> GetAggregatedValueForIndirectMainAssignments(IEnumerable<ChangedValue> changedValues)
    {
        var changedValuesGroupBy = changedValues.GroupBy(v => new AssignmentGroup
        {
            MatchId = v.MatchId,
            BetTypeId = v.BetTypeId,
            AssigneeId = v.AssigneeId,
            IsWatcher = v.IsWatcher,
            IsIndirectChanged = v.IsIndirectChangedAssignment
        });

        foreach (var item in changedValuesGroupBy)
        {
            var deletingAssignment = item.FirstOrDefault(m => m.MessageTypeId == (int)MsgTemplateCode.AssignmentDelete);
            var insertingAssignment = item.FirstOrDefault(m => m.MessageTypeId == (int)MsgTemplateCode.AssignmentInsert);

            if (deletingAssignment != null && insertingAssignment != null)
            {
                yield return GetAggregatedValueForDoubleAssignment(deletingAssignment, insertingAssignment);
            }

            if (deletingAssignment != null && insertingAssignment == null)
            {
                yield return GetAggregatedValueForSingleAssignment(deletingAssignment);
            }

            if (insertingAssignment != null && deletingAssignment == null)
            {
                yield return GetAggregatedValueForSingleAssignment(insertingAssignment);
            }
        }
    }

    public ChangedValue GetAggregatedValueForDoubleAssignment(ChangedValue deletingAssignment, ChangedValue insertingChangedValue)
    {
        var aggregatedChangedValue = mapper.Map<ChangedValue>(deletingAssignment);

        aggregatedChangedValue.MessageTypeId = (int)MsgTemplateCode.ChangeOfAssignment;
        aggregatedChangedValue.OldValue = deletingAssignment.OldValue;
        aggregatedChangedValue.NewValue = insertingChangedValue.NewValue;

        return aggregatedChangedValue;
    }

    private ChangedValue GetAggregatedValueForSingleAssignment(ChangedValue changedValue)
    {
        var aggregatedChangedValue = mapper.Map<ChangedValue>(changedValue);
        aggregatedChangedValue.MessageTypeId = (int)MsgTemplateCode.ChangeOfAssignment;
        return aggregatedChangedValue;
    }

    public static IEnumerable<ChangedValue> GetAggregatedValueForIndirectOtherAssignments(IEnumerable<ChangedValue> changedValues)
    {
        var changedValueList = changedValues.ToList();
        foreach (var value in changedValueList)
        {
            value.MessageTypeId = (int)MsgTemplateCode.ChangeOfAssignment;
        }

        return changedValueList;
    }
}