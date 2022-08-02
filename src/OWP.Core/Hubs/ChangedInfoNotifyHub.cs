using Microsoft.AspNetCore.SignalR;

namespace OWP.Hubs;


public interface ChangedInfoNotifyHub
{
    Task notifyChangedInformationData(dynamic message);
}

public class ChangedInfoNotifyHubImp : Hub<ChangedInfoNotifyHub>
{
}