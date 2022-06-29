namespace OWP.PreMatchNotification.LinkedMatch.Services;

using Models;
using OWP.PreMatchNotification.Shared.Services;

public interface CacheManipulator
{
    Task<List<GeneralMatch>> GetOldLGAMatchSnapshot(string sessionId);

    Task DeleteAssignmentSnapshot(string sessionId);
}

public class CacheManipulatorImp : CacheManipulator
{
    private readonly CacheManipulatorCommon cacheManipulator;
    private const string OLD_LGAMATCHES_SNAPSHOT_KEY = "OWP-ASSIGNMENT-SNAPSHOT";

    public CacheManipulatorImp(CacheManipulatorCommon cacheManipulator)
    {
        this.cacheManipulator = cacheManipulator;
    }

    public async Task<List<GeneralMatch>> GetOldLGAMatchSnapshot(string sessionId)
    {
        var cacheKey = $"{OLD_LGAMATCHES_SNAPSHOT_KEY}-{sessionId}";
        var assignmentSnapshot = await cacheManipulator.GetSingleData(cacheKey, new AssignmentSnapshot());

        return assignmentSnapshot.Matches.Values.ToList();
    }

    public async Task DeleteAssignmentSnapshot(string sessionId)
    {
        var cacheKey = $"{OLD_LGAMATCHES_SNAPSHOT_KEY}-{sessionId}";
        await cacheManipulator.RemoveData(cacheKey);
    }
}