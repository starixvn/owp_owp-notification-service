namespace OWP.AssignmentUploadNotification.Extensions
{
    using System.Text.RegularExpressions;

    public static class UpdatedValueHelper
    {
        private const string USER = "#<user>";

        public static string GetValue(int assignmentType, string userName, string assignmentNote)
        {
            return assignmentType == 1 ? $"#{userName}" : GetAssignmentNote(assignmentNote, userName);
        }

        private static string GetAssignmentNote(string assignmentNote, string userName)
        {
            var newValue = string.IsNullOrEmpty(userName)
                            ? Regex.Replace(assignmentNote, USER, "", RegexOptions.IgnoreCase)
                            : Regex.Replace(assignmentNote, USER, $"#{userName}", RegexOptions.IgnoreCase);
            newValue = Regex.Replace(newValue, "same", "", RegexOptions.IgnoreCase).Trim();
            return newValue;
        }
    }
}
