using OWP.PreMatchNotification.LinkedMatch.Models;
using OWP.PreMatchNotification.Shared.Services;

namespace OWP.PreMatchNotification.LinkedMatch.Services;
public interface CacheManipulator
{
    Task<List<GeneralMatch>> GetOldLGAMatchSnapshot(string sessionId);

    Task DeleteAssignmentSnapshot(string sessionId);
}

public class CacheManipulatorImp(CacheManipulatorCommon cacheManipulator) : CacheManipulator
{
    private const string OldLgaMatchesSnapshotKey = "OWP-ASSIGNMENT-SNAPSHOT";

    public async Task<List<GeneralMatch>> GetOldLGAMatchSnapshot(string sessionId)
    {
        var cacheKey = $"{OldLgaMatchesSnapshotKey}-{sessionId}";
        var assignmentSnapshot = await cacheManipulator.GetSingleData(cacheKey, new AssignmentSnapshot());

        return assignmentSnapshot.Matches.Values.ToList();
    }

    public async Task DeleteAssignmentSnapshot(string sessionId)
    {
        var cacheKey = $"{OldLgaMatchesSnapshotKey}-{sessionId}";
        await cacheManipulator.RemoveData(cacheKey);
    }
}