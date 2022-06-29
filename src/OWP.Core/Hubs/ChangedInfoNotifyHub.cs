using Microsoft.AspNetCore.SignalR;

namespace OWP.Core.Hubs;


public interface ChangedInfoNotifyHub
{
    Task notifyChangedInformationData(dynamic message);
}

public class ChangedInfoNotifyHubImp : Hub<ChangedInfoNotifyHub>
{
}