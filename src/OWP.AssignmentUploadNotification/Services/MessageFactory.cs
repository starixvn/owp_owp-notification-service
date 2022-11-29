namespace OWP.AssignmentUploadNotification.Services
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Constants;
    using Models;

    public interface MessageFactory
    {
        List<MessageModel> Transform(List<ChangedValue> changedValues, string userName);
    }

    public class MessageFactoryImp : MessageFactory
    {
        #region MessageTemplate

        private static readonly ImmutableList<MessageTemplate> messageTemplates = ImmutableList<MessageTemplate>.Empty.AddRange(new[]
        {
            new MessageTemplate
            {
                MessageType = (int)MsgTemplateCode.AssignmentDelete,
                MessageContent = "Your assignment has been REMOVED <Bettype> for the match: <Match.No> - <League> - <Event> updated by <User>",
                IsImportant = true
            },

            new MessageTemplate
            {
                MessageType = (int)MsgTemplateCode.AssignmentInsert,
                MessageContent = "You have been new ASSIGNED <Bettype> for the match: <Match.No> - <League> - <Event> updated by <User>",
                IsImportant = true
            },

            new MessageTemplate
            {
                MessageType = (int)MsgTemplateCode.ChangeOfAssignment,
                MessageContent = "Was updated <Bettype> from [ <Old> ] change to [ <New> ]  for the match: <Match.No> - <League> - <Event> updated by <User>"
            },

            new MessageTemplate
            {
                MessageType = (int)MsgTemplateCode.EditAgent,
                MessageContent = "Was updated Agent: [ <New> ]  for the match: <Match.No> - <League> - <Event> updated by <User>, previous: [ <Old> ]"
            },

            new MessageTemplate
            {
                MessageType = (int)MsgTemplateCode.EditChannel,
                MessageContent = "Was updated IPTV/channel: [ <New> ]  for the match: <Match.No> - <League> - <Event> updated by <User>, previous: [ <Old> ]"
            },

            new MessageTemplate
            {
                MessageType = (int)MsgTemplateCode.EditDivision,
                MessageContent = "Was updated Division: [ <New> ]  for the match: <Match.No> - <League> - <Event> updated by <User>, previous: [ <Old> ]"
            },

            new MessageTemplate
            {
                MessageType = (int)MsgTemplateCode.EditKOTime,
                MessageContent = "Was updated KO Time: [ <New> ]  for the match: <Match.No> - <League> - <Event> updated by <User>, previous: [ <Old> ]"
            },

            new MessageTemplate
            {
                MessageType = (int)MsgTemplateCode.EditMatchNo,
                MessageContent = "Was updated match: #<New> - <League> - <Event> updated by <User>, previous: [ #<Old> - <League> - <Event> ]"
            },

            new MessageTemplate
            {
                MessageType = (int)MsgTemplateCode.EditMatchOffer,
                MessageContent = "Was updated Offer: [ <New> ]  for the match: <Match.No> - <League> - <Event> updated by <User>, previous: [ <Old> ]"
            },

            new MessageTemplate
            {
                MessageType = (int)MsgTemplateCode.EditNoOfHDP,
                MessageContent = "Was updated No of HDP: [ <New> ]  for the match: <Match.No> - <League> - <Event> updated by <User>, previous: [ <Old> ]"
            },

            new MessageTemplate
            {
                MessageType = (int)MsgTemplateCode.EditOtherProvider,
                MessageContent = "Was updated Other Provider: [ <New> ]  for the match: <Match.No> - <League> - <Event> updated by <User>, previous: [ <Old> ]"
            },

            new MessageTemplate
            {
                MessageType = (int)MsgTemplateCode.EditOtherTime,
                MessageContent = "Was updated Other Time: [ <New> ]  for the match: <Match.No> - <League> - <Event> updated by <User>, previous: [ <Old> ]"
            },

            new MessageTemplate
            {
                MessageType = (int)MsgTemplateCode.EditPhone,
                MessageContent = "Was updated Phone: [ <New> ]  for the match: <Match.No> - <League> - <Event> updated by <User>, previous: [ <Old> ]"
            },

            new MessageTemplate
            {
                MessageType = (int)MsgTemplateCode.EditProvider,
                MessageContent = "Was updated Provider: [ <New> ]  for the match: <Match.No> - <League> - <Event> updated by <User>, previous: [ <Old> ]"
            },

            new MessageTemplate
            {
                MessageType = (int)MsgTemplateCode.EditRemark,
                MessageContent = "Was updated Remark: [ <New> ]  for the match: <Match.No> - <League> - <Event> updated by <User>, previous: [ <Old> ]"
            },

            new MessageTemplate
            {
                MessageType = (int)MsgTemplateCode.EditRound,
                MessageContent = "Was updated Round: [ <New> ]  for the match: <Match.No> - <League> - <Event> updated by <User>, previous: [ <Old> ]"
            },

            new MessageTemplate
            {
                MessageType = (int)MsgTemplateCode.EditTV,
                MessageContent = "Was updated TV: [ <New> ]  for the match: <Match.No> - <League> - <Event> updated by <User>, previous: [ <Old> ]"
            },

            new MessageTemplate
            {
                MessageType = (int)MsgTemplateCode.EditWB,
                MessageContent = "Was updated WB: [ <New> ]  for the match: <Match.No> - <League> - <Event> updated by <User>, previous: [ <Old> ]"
            },

            new MessageTemplate
            {
                MessageType = (int)MsgTemplateCode.EditWebsite1,
                MessageContent = "Was updated Website1: [ <New> ]  for the match: <Match.No> - <League> - <Event> updated by <User>, previous: [ <Old> ]"
            },

            new MessageTemplate
            {
                MessageType = (int)MsgTemplateCode.EditWebsite2,
                MessageContent = "Was updated Website2: [ <New> ]  for the match: <Match.No> - <League> - <Event> updated by <User>, previous: [ <Old> ]"
            },

            new MessageTemplate
            {
                MessageType = (int)MsgTemplateCode.EditWebsite3,
                MessageContent = "Was updated Website3: [ <New> ]  for the match: <Match.No> - <League> - <Event> updated by <User>, previous: [ <Old> ]"
            },

            new MessageTemplate
            {
                MessageType = (int)MsgTemplateCode.EditWebsite4,
                MessageContent = "Was updated Website4: [ <New> ]  for the match: <Match.No> - <League> - <Event> updated by <User>, previous: [ <Old> ]"
            },

            new MessageTemplate
            {
                MessageType = (int)MsgTemplateCode.EditMaxbet,
                MessageContent = "Was updated Maxbet of <Bettype>: [ <New> ]  for the match: <Match.No> - <League> - <Event> updated by <User>, previous: [ <Old> ]"
            },

            new MessageTemplate
            {
                MessageType = (int)MsgTemplateCode.EditSpread,
                MessageContent = "Was updated Spread of <Bettype>: [ <New> ]  for the match: <Match.No> - <League> - <Event> updated by <User>, previous: [ <Old> ]"
            },

            new MessageTemplate
            {
                MessageType = (int)MsgTemplateCode.MatchStatus,
                MessageContent = "Has been <New> for the match: <Match.No> - <League> - <Event> updated by <User>",
                IsImportant = true
            },

            new MessageTemplate
            {
                MessageType = (int)MsgTemplateCode.MinorMatchStatus,
                MessageContent = "Has been <New> bettype: <Bettype> for the match: <Match.No> - <League> - <Event> updated by <User>",
                IsImportant = true
            },

            new MessageTemplate
            {
                MessageType = (int)MsgTemplateCode.OtherAssignmentDelete,
                MessageContent = "You assignment has been REMOVED <Bettype> <Note> for the match: <Match.No> - <League> - <Event> updated by <User>",
                IsImportant = true
            },

            new MessageTemplate
            {
                MessageType = (int)MsgTemplateCode.OtherAssignmentInsert,
                MessageContent = "You have been new ASSIGNED <Bettype> <Note> for the match: <Match.No> - <League> - <Event> updated by <User>",
                IsImportant = true
            },

            new MessageTemplate
            {
                MessageType = (int)MsgTemplateCode.OtherAssignmentUpdate,
                MessageContent = "Was updated <Bettype> Other Assign: [ <New> ] for the match: <Match.No> - <League> - <Event> updated by <User>, previous: [ <Old> ]",
                IsImportant = true
            },

            new MessageTemplate
            {
                MessageType = (int)MsgTemplateCode.EditHomeName,
                MessageContent = "Was updated match: <Match.No> - <League> - <New> updated by <User>, previous: [ <Match.No> - <League> - <Old> ]"
            }
        });

        #endregion MessageTemplate

        #region MessageGroup

        private class MessageGroup : IEqualityComparer<MessageGroup>
        {
            public MessageGroup(ChangedValue value)
            {
                AssignmentNote = value.AssignmentNote;
                BetTypeId = value.BetTypeId;
                BetTypeName = value.BetTypeName;
                MessageTypeId = value.MessageTypeId;
                NewValue = value.NewValue;
                OldValue = value.OldValue;
                League = value.League;
                EventName = value.EventName;
                MatchNo = value.MatchNo;
                IsWatcher = value.IsWatcher;
            }

            public string AssignmentNote { get; set; }
            public int BetTypeId { get; set; }

            public string BetTypeName { get; set; }

            public int MessageTypeId { get; set; }

            public string NewValue { get; set; }

            public string OldValue { get; set; }

            public string League { get; set; }

            public string EventName { get; set; }

            public string MatchNo { get; set; }

            public bool IsWatcher { get; set; }

            public bool Equals(MessageGroup x, MessageGroup y)
            {
#pragma warning disable S1067 // Expressions should not be too complex - ignored by Jacob
                return x.AssignmentNote == y.AssignmentNote &&
                   x.BetTypeId == y.BetTypeId &&
                   x.BetTypeName == y.BetTypeName &&
                   x.MessageTypeId == y.MessageTypeId &&
                   x.NewValue == y.NewValue &&
                   x.OldValue == y.OldValue &&
                   x.League == y.League &&
                   x.EventName == y.EventName &&
                   x.MatchNo == y.MatchNo &&
                   x.IsWatcher == y.IsWatcher;
#pragma warning restore S1067 // Expressions should not be too complex
            }

            public int GetHashCode(MessageGroup obj)
            {
                var hash = 17;
                hash = hash * 23 + obj.BetTypeId;
                hash = hash * 31 + obj.MessageTypeId;
                hash = hash * 47 + obj.League.GetHashCode();
                hash = hash * 59 + obj.EventName.GetHashCode();

                return hash;
            }
        }

        #endregion MessageGroup

        #region MessageModelRaw

        private class MessageModelRaw
        {
            public string MessageContent { get; set; }

            public bool IsImportant { get; set; }

            public bool IsWatcher { get; set; }

            public int MessageType { get; set; }

            public List<Assignee> Assignees { get; set; }
        }

        #endregion MessageModelRaw

        private static readonly Dictionary<int, MessageTemplate> messageTemplateMap = messageTemplates.ToDictionary(m => m.MessageType, m => m);

        public List<MessageModel> Transform(List<ChangedValue> changedValues, string userName)
        {
            var messageModels = TransformChangedValueToMessageModel(changedValues, userName);
            return Reduce(messageModels);
        }

        private List<MessageModelRaw> TransformChangedValueToMessageModel(List<ChangedValue> changedValues, string userName)
        {
            return changedValues.GroupBy(v => new MessageGroup(v))
                .Select(g =>
                {
                    var messageTemplate = messageTemplateMap[g.Key.MessageTypeId];
                    var content = CreateMessageContent(g.Key, userName);

                    return new MessageModelRaw
                    {
                        IsImportant = messageTemplate.IsImportant,
                        MessageContent = content,
                        IsWatcher = g.Key.IsWatcher,
                        MessageType = g.Key.MessageTypeId,
                        Assignees = g.Select(i => new Assignee { AssigneeId = i.AssigneeId, AssigneeName = i.AssigneeName }).ToList()
                    };
                }).ToList();
        }

        private string CreateMessageContent(MessageGroup messageGroup, string userName)
        {
            var messageTemplate = messageTemplateMap[messageGroup.MessageTypeId];
            var content = messageTemplate.MessageContent;

            var assignmentNote = Regex.Replace(messageGroup.AssignmentNote, "#<user>", "", RegexOptions.IgnoreCase);
            assignmentNote = Regex.Replace(assignmentNote, "same", "", RegexOptions.IgnoreCase);

            content = content.Replace("<New>", messageGroup.NewValue);
            content = content.Replace("<Match.No>", !string.IsNullOrEmpty(messageGroup.MatchNo) ? $"#{messageGroup.MatchNo}" : string.Empty);
            content = content.Replace("<League>", messageGroup.League);
            content = content.Replace("<Event>", messageGroup.EventName);
            content = content.Replace("<User>", userName);
            content = content.Replace("<Old>", messageGroup.OldValue);
            content = content.Replace("<Bettype>", messageGroup.BetTypeName);
            content = content.Replace("<Note>", assignmentNote);

            return content;
        }

        private static List<MessageModel> Reduce(List<MessageModelRaw> messageModels)
        {
            return messageModels.GroupBy(m => new { m.MessageContent, m.IsImportant, m.IsWatcher, m.MessageType })
                .Select(m =>
                {
                    var assignees = m.SelectMany(x => x.Assignees).Distinct().ToArray();
                    return new MessageModel
                    {
                        MessageType = m.Key.MessageType,
                        MessageContent = m.Key.MessageContent,
                        IsImportant = m.Key.IsImportant,
                        IsWatcher = m.Key.IsWatcher,
                        ReceivedUserIdList = string.Join(",", assignees.Select(a => a.AssigneeId)),
                        ReceivedUserNameList = string.Join(",", assignees.Select(a => a.AssigneeName))
                    };
                })
                .ToList();
        }
    }
}