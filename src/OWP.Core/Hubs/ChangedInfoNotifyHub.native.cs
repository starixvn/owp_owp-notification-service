

// ReSharper disable once CheckNamespace
namespace OWP.Hubs;

using Microsoft.AspNetCore.SignalR; // using the same namespace for RedisBackPlane


public interface ChangedInfoNotifyHub
{
    Task notifyChangedInformationData(dynamic message);
}

public class ChangedInfoNotifyHubImp : Hub<ChangedInfoNotifyHub>;