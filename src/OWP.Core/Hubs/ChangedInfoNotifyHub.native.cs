using Microsoft.AspNetCore.SignalR;

// ReSharper disable once CheckNamespace
namespace OWP.Hubs; // using the same namespace for RedisBackPlane


public interface ChangedInfoNotifyHub
{
    Task notifyChangedInformationData(dynamic message);
}

public class ChangedInfoNotifyHubImp : Hub<ChangedInfoNotifyHub>
{
}