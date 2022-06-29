using Microsoft.AspNetCore.SignalR;

namespace OWP.NotificationHandler.Hubs;


public interface ChangedInfoNotifyHub
{
    Task notifyChangedInformationData(object message);
}

public class ChangedInfoNotifyHubImp : Hub<ChangedInfoNotifyHub>
{

}