using OWP.PreMatchNotification.LinkedMatch.Models;
using OWP.PreMatchNotification.Shared.Models;

namespace OWP.PreMatchNotification.LinkedMatch.Services;

public interface ChangedLinkedMatchDetector
{
    List<ChangedValue> Create(List<GeneralMatch> oldMatches, List<GeneralMatch> newMatches, Employee employees);
}

public class ChangedLinkedMatchDetectorImp(
    ChangedLiveMatchGenerator changedLiveMatchGenerator,
    ChangedMatchStatusGenerator changedMatchStatusGenerator)
    : ChangedLinkedMatchDetector
{
    public List<ChangedValue> Create(List<GeneralMatch> oldMatches, List<GeneralMatch> newMatches, Employee employees)
    {
        var changedSnapshot = GetChangedSnapshot(oldMatches, newMatches, employees);

        var liveMatchChangedValue = changedLiveMatchGenerator.Create(changedSnapshot, employees);
        var matchStatusChangedValue = changedMatchStatusGenerator.Create(changedSnapshot, employees);

        return liveMatchChangedValue.Union(matchStatusChangedValue).ToList();
    }

    public ChangedSnapshot GetChangedSnapshot(List<GeneralMatch> oldMatches, List<GeneralMatch> newMatches, Employee employees)
    {
        var filteredOldMatches = FilterExistPrematches(oldMatches, employees.PrematchInfosites);
        var filteredNewMatches = FilterExistPrematches(newMatches, employees.PrematchInfosites);

        var deletedMatches = GetDeletedLinkedMatches(filteredOldMatches, filteredNewMatches);
        var insertedMatches = GetInsertedLinkedMatches(filteredOldMatches, filteredNewMatches);
        var changedMatches = GetChangedLinkedMatches(filteredOldMatches, filteredNewMatches);

        return new ChangedSnapshot
        {
            DeletedMatches = deletedMatches,
            InsertedMatches = insertedMatches,
            ChangedMatches = changedMatches,
            OldSnapshot = oldMatches,
            NewSnapshot = newMatches
        };
    }

    public static List<GeneralMatch> FilterExistPrematches(List<GeneralMatch> matches, Dictionary<int, List<InfositePrematchLeague>> prematchLeagues)
    {
        var existInfositePrematch = prematchLeagues.Keys.Distinct();
        return matches.Where(x => x.InfoSiteMatchId != 0 && existInfositePrematch.Contains(x.InfoSiteMatchId)).ToList();
    }

    public static List<GeneralMatch> GetDeletedLinkedMatches(IEnumerable<GeneralMatch> oldMatches, IEnumerable<GeneralMatch> newMatches)
    {
        return oldMatches.Except(newMatches, new LinkedMatchComparer()).ToList();
    }

    public static List<GeneralMatch> GetInsertedLinkedMatches(IEnumerable<GeneralMatch> oldMatches, IEnumerable<GeneralMatch> newMatches)
    {
        return newMatches.Except(oldMatches, new LinkedMatchComparer()).ToList();
    }

    public static List<GeneralMatch> GetChangedLinkedMatches(IEnumerable<GeneralMatch> oldMatches, IEnumerable<GeneralMatch> newMatches)
    {
        return oldMatches.Intersect(newMatches, new LinkedMatchComparer()).ToList();
    }

    internal class LinkedMatchComparer : IEqualityComparer<GeneralMatch>
    {
        public bool Equals(GeneralMatch x, GeneralMatch y)
        {
            return x.InfoSiteMatchId == y.InfoSiteMatchId &&
                   x.MatchId == y.MatchId;
        }

        public int GetHashCode(GeneralMatch obj)
        {
            return obj.MatchId.GetHashCode();
        }
    }
}