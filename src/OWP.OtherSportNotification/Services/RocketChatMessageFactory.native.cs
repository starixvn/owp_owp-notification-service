using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using Microsoft.Extensions.Configuration;
using Olympus.OWP.Log.Data.Repository.Criteria;
using OWP.OtherSportNotification.Constants;
using OWP.OtherSportNotification.Models;

namespace OWP.OtherSportNotification.Services
{
    public interface RocketChatMessageFactory
    {
        Task<IList<RocketChatMessageModel>> BuildMessage(IList<ChangedValue> changedValues);
    }

    public class RocketChatMessageFactoryImp(OtherSportRepository repository, IConfiguration configuration) : RocketChatMessageFactory
    {
       
        private readonly Dictionary<int, MessageTemplate> messageTemplateMap = ImmutableList<MessageTemplate>.Empty.AddRange(
        [
            new MessageTemplate
            {
                MessageType = (int)RocketChatMsgTemplateCode.AssignmentInsert,
                MessageContent = $"Hi <tradername>,\nYou've been assigned to new matches.\n[View details here]({configuration["OwpUrl"]})\nThanks."
            },
            new MessageTemplate
            {
                MessageType = (int)RocketChatMsgTemplateCode.AssignmentDelete,
                MessageContent = $"Hi <tradername>,\nYou've been removed from some matches.\n[View details here]({configuration["OwpUrl"]})\nThanks."
            },
            new MessageTemplate
            {
                MessageType = (int)RocketChatMsgTemplateCode.AssignmentInsertAndDelete,
                MessageContent = $"Hi <tradername>,\nSome matches have been reassigned. You've been both removed and assigned.\n[View details here]({configuration["OwpUrl"]})\nThanks."
            }
        ]).
            ToDictionary(m => m.MessageType, m => m);

        public async Task<IList<RocketChatMessageModel>> BuildMessage(IList<ChangedValue> changedValues)
        {
            var message = new List<RocketChatMessageModel>();
            try
            {
                var changedValueMapUsername = changedValues
                    .Where(c=> CanSendMessageToTraderCaseInsert(c) || CanSendMessageToTraderCaseRemove(c))
                    .GroupBy(c => c.Username)
                    .ToDictionary(x => x.Key, x => x.ToList());

                message = (
                    from changeValue in changedValueMapUsername
                    let msgTemplateCode = DetectChatMsgTemplateCode(ExistsInsertAssignee(changeValue.Value),
                        ExistsDeleteAssignee(changeValue.Value))
                    select new RocketChatMessageModel()
                    {
                        Message = messageTemplateMap[(int)msgTemplateCode].MessageContent
                            .Replace("<tradername>", changeValue.Key),
                        Recipient = changeValue.Key
                    }).ToList();
                return message;

            }
            catch (Exception ex)
            {
                await LogDetailedPayload(ToGmt8(DateTime.Now), $"[RocketChatMessageModel] - Exception", ex.Message);
                return [];
            }
            finally
            {
                await LogDetailedPayload(ToGmt8(DateTime.Now), $"[RocketChatMessageModel] - msgModel",
                    System.Text.Json.JsonSerializer.Serialize(new
                    {
                        count=message.Count,
                        message= message

                    }));
            }
        }
       
        private static RocketChatMsgTemplateCode DetectChatMsgTemplateCode(bool existsInsertAssignee, bool existsDeleteAssignee)
        {
            if (existsInsertAssignee && existsDeleteAssignee)
            {
                return RocketChatMsgTemplateCode.AssignmentInsertAndDelete;
            }
            if (existsInsertAssignee)
            {
                return RocketChatMsgTemplateCode.AssignmentInsert;
            }
            if (existsDeleteAssignee)
            {
                return RocketChatMsgTemplateCode.AssignmentDelete;
            }

            throw new InvalidOperationException("No valid message type found.");
        }

        private static bool ExistsInsertAssignee(IList<ChangedValue> changedValues)
            => changedValues.Any(CanSendMessageToTraderCaseInsert);

        private static bool ExistsDeleteAssignee(IList<ChangedValue> changedValues)
            => changedValues.Any(CanSendMessageToTraderCaseRemove);

        private static bool CanSendMessageToTraderCaseInsert(ChangedValue changedValue)
        {
            return changedValue.MessageTypeId is
                (int)MsgTemplateCode.AssignmentInsert
                or (int)MsgTemplateCode.OtherAssignmentInsert;

        }

        private static bool CanSendMessageToTraderCaseRemove(ChangedValue changedValue)
        {
            return changedValue.MessageTypeId is
                (int)MsgTemplateCode.AssignmentDelete
                or (int)MsgTemplateCode.OtherAssignmentDelete;

        }

        private async Task LogDetailedPayload(DateTime CreatedDate, string Action, string Payload)
        {
            await repository.WriteLog(new OwpLogCriteria()
            {
                CreatedDate = CreatedDate,
                Action = Action,
                Payload = Payload
            });
        }

        private static DateTime ToGmt8(DateTime dateTime)
        {
            return dateTime.ToUniversalTime().AddHours(8);
        }
    }
}
