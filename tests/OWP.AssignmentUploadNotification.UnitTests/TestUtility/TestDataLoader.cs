namespace OWP.AssignmentUploadNotification.UnitTests.TestUtility
{
    using System;
    using System.IO;
    using Newtonsoft.Json;

    internal static class TestDataLoader
    {
        internal static T Load<T>(string filename)
        {
            var directory = $"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}TestData";
            var dataFilePath = $"{directory}{Path.DirectorySeparatorChar}{filename}";
            var text = File.ReadAllText(dataFilePath);

            return JsonConvert.DeserializeObject<T>(text);
        }
    }
}