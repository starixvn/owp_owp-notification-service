namespace OWP.AssignmentUploadNotification.UnitTests.Notification.Sevices
{
    using System.Collections.Generic;
    using OWP.AssignmentUploadNotification.Models;
    using OWP.AssignmentUploadNotification.Services;
    using Xunit;
    using Xunit.Asserts.Compare;

    public class MessageFactoryUnitTest
    {
        private readonly MessageFactory messageFactory;

        public MessageFactoryUnitTest()
        {
            messageFactory = new MessageFactoryImp();
        }

        [Fact]

        public void Transform_Always_ReturnMessageList()
        {
            var changedValueList = GetListChangedValue();

            var expected = new List<MessageModel>
            {
                new ()
                {
                   IsImportant = false,
                   MessageContent = "Was updated match: #20000 - A-Coners - LiverPool-Coners -vs- Arsenal-Coners updated by sa, previous: [ #1234 - A-Coners - LiverPool-Coners -vs- Arsenal-Coners ]",
                   MessageType = 40,
                   IsWatcher = false,
                    ReceivedUserIds= new List<string>{"6789"},
                   ReceivedUserIdList = "6789",
                   ReceivedUserNameList = "esther"
                },

                new MessageModel
                {
                   IsImportant = false,
                   MessageContent = "Was updated Maxbet of Next of Coners (HDP): [ 300K ]  for the match: #50000 - PremierLeague - LiverPool -vs- Arsenal updated by sa, previous: [ 400K ]",
                   MessageType = 29,
                   IsWatcher = false,
                   ReceivedUserIds= new List<string>{"6789"},
                   ReceivedUserIdList = "6789",
                   ReceivedUserNameList = "esther"
                }
            };

            var actual = messageFactory.Transform(changedValueList, "sa");
            DeepAssert.Equal(expected, actual);
        }

        [Fact]

        public void Transform_OtherAssignmentChanged_ReturnMessageList()
        {
            var changedValueList = GetListAssignmentChanged();

            var expected = new List<MessageModel>
            {
                new ()
                {
                   IsImportant = true,
                   MessageContent = "You assignment has been REMOVED HDP PK  for the match:  - Jo - Spur -vs- MC updated by sa",
                   MessageType = 9,
                   IsWatcher = false,
                   ReceivedUserIds= new List<string>{"2542"},
                   ReceivedUserIdList = "2542",
                   ReceivedUserNameList = "esther"
                },

                new MessageModel
                {
                   IsImportant = true,
                   MessageContent = "You have been new ASSIGNED 1H HDP  for the match: #5000 - Jo - Spur -vs- MC updated by sa",
                   MessageType = 8,
                   IsWatcher = false,
                     ReceivedUserIds= new List<string>{"2542"},
                   ReceivedUserIdList = "2542",
                   ReceivedUserNameList = "esther"
                }
            };

            var actual = messageFactory.Transform(changedValueList, "sa");
            DeepAssert.Equal(expected, actual);
        }

        private List<ChangedValue> GetListAssignmentChanged()
        {
            var valueChanged = new ChangedValue
            {
                MessageTypeId = 9,
                BetTypeId = 1,
                OldValue = "PK Same",
                NewValue = null,
                AssignmentNote = "PK Same",
                AssignmentType = 2,
                BetTypeName = "HDP",
                MatchId = 5567,
                MatchNo = "",
                League = "Jo",
                EventName = "Spur -vs- MC",
                AssigneeId = 2542,
                AssigneeName = "esther"
            };

            var valueChanged2 = new ChangedValue
            {
                MessageTypeId = 8,
                BetTypeId = 1,
                OldValue = null,
                NewValue = "#esther",
                AssignmentNote = "#<user>",
                AssignmentType = 2,
                BetTypeName = "1H HDP",
                MatchId = 5567,
                MatchNo = "5000",
                League = "Jo",
                EventName = "Spur -vs- MC",
                AssigneeId = 2542,
                AssigneeName = "esther"
            };

            var changedValueList = new List<ChangedValue>
            {
                valueChanged,
                valueChanged2
            };

            return changedValueList;
        }
        private List<ChangedValue> GetListChangedValue()
        {
            var valueChanged = new ChangedValue
            {
                MessageTypeId = 40,
                NewValue = "20000",
                OldValue = "1234",
                League = "A-Coners",
                MatchNo = "20000",
                EventName = "LiverPool-Coners -vs- Arsenal-Coners",
                AssigneeId = 6789,
                AssigneeName = "esther"
            };

            var valueChanged2 = new ChangedValue
            {
                OldValue = "400K",
                NewValue = "300K",
                BetTypeName = "Next of Coners (HDP)",
                MatchNo = "50000",
                League = "PremierLeague",
                EventName = "LiverPool -vs- Arsenal",
                AssigneeId = 6789,
                AssigneeName = "esther",
                MessageTypeId = 29
            };

            var changedValueList = new List<ChangedValue>
            {
                valueChanged,
                valueChanged2
            };

            return changedValueList;
        }
    }
}
