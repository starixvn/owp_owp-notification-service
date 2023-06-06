using OWP.OtherSportNotification.Models;
using System.Text.RegularExpressions;

namespace OWP.OtherSportNotification.Helpers;

public static class UpdatedValueHelper
{
    private const string USER = "#<user>";

    public static string GetValue(string userName, string assignmentNote)
    {
        var newValue = string.IsNullOrEmpty(userName)
            ? Regex.Replace(assignmentNote, USER, "", RegexOptions.IgnoreCase)
            : Regex.Replace(assignmentNote, USER, $"#{userName}", RegexOptions.IgnoreCase);
        return newValue;
    }

    public static string CreateEventName(MatchInfo match) => (match.IsNeutral ? $"{match.Home} (N)" : match.Home) + " -vs- " + match.Away;
}