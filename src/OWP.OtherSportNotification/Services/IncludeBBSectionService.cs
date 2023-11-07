using Fanex.Logging;
using OWP.Core.NotificationRequests.OtherSport;
using OWP.OtherSportNotification.Constants;
using OWP.OtherSportNotification.Models;

namespace OWP.OtherSportNotification.Services;

public interface IncludeBBSectionService
{
    Task InsertMessage(List<ChangedValue> changedValues, OtherSportNotificationRequest request);
}

public class IncludeBBSectionServiceImp : IncludeBBSectionService
{
    private readonly OtherSportRepository repository;

    private readonly MessageFactory messageFactory;

    private readonly ILogger logger;

    private static readonly List<int> ActionsInclude = new(){
       (int)MsgTemplateCode.EditMatchNo      ,
       (int)MsgTemplateCode.EditMatchStatus  ,
       (int)MsgTemplateCode.EditCategory     ,
       (int)MsgTemplateCode.EditNoOfHDP      ,
       (int)MsgTemplateCode.EditMatchOffer   ,
       (int)MsgTemplateCode.EditAutoLink     ,
       (int)MsgTemplateCode.EditOtherProvider,
       (int)MsgTemplateCode.EditIPTV         ,
       (int)MsgTemplateCode.EditWB           ,
       (int)MsgTemplateCode.EditRemark       ,
       (int)MsgTemplateCode.EditMaxbet       ,
       (int)MsgTemplateCode.EditReporter     ,
       (int)MsgTemplateCode.EditProvider     ,
       (int)MsgTemplateCode.EditAssignmentNote ,
       (int)MsgTemplateCode.EditSpread       ,
       (int)MsgTemplateCode.EditNeutral      ,
    };

    public IncludeBBSectionServiceImp(OtherSportRepository repository, MessageFactory messageFactory, ILogger logger)
    {
        this.repository = repository;
        this.messageFactory = messageFactory;
        this.logger = logger;
    }

    public async Task InsertMessage(List<ChangedValue> changedValues, OtherSportNotificationRequest request)
    {
        var bbSupportiveSection = await repository.QuerySectionBBSupportive();
        var messageTarget = FilterChangedValues(changedValues);
        var messages = messageFactory.Transform(messageTarget, request.UserName); 

        messages.ForEach(message =>
        {
            message.ReceivedUserIds.Clear();
            message.ReceivedUserIds.AddRange(bbSupportiveSection);
        });
        
        await logger.InfoAsync("BBSupportive section");
        await repository.InsertMessage(messages, request.SessionId, request.ActorId);
    }

    private static List<ChangedValue> FilterChangedValues(List<ChangedValue> changedValues)
        => changedValues.Where(c=>ActionsInclude.Exists(action => action == c.MessageTypeId)).ToList();
}
