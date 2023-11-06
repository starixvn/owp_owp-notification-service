using OWP.OtherSportNotification.Constants;
using OWP.OtherSportNotification.Models;

namespace OWP.OtherSportNotification.Services;

public interface IncludeBBSectionService
{
    Task Execute(List<MessageModel> messages);
}

public class IncludeBBSectionServiceImp : IncludeBBSectionService
{
    private readonly OtherSportRepository repository;

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

    public IncludeBBSectionServiceImp(OtherSportRepository repository)
    {
        this.repository = repository;
    }

    public async Task Execute(List<MessageModel> messages)
    {
        var bbSupportiveSection = await repository.QuerySectionBBSupportive();
        messages.ForEach(message =>
         {
             if (ActionsInclude.Exists(action => action == message.MessageType))
             {
                 message.ReceivedUserIds.AddRange(bbSupportiveSection);
             }
         });
    }
}
