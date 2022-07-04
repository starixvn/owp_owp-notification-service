using Microsoft.Extensions.DependencyInjection;
using MyAssignment = OWP.PreMatchNotification.Assignment.Services.MyAssignment;
using OtherAssignment = OWP.PreMatchNotification.Assignment.Services.OtherAssignment;
using PreMatchNotificationServices = OWP.PreMatchNotification.Shared.Services;
using LinkedMatchs = OWP.PreMatchNotification.LinkedMatch.Services;
using InfoSiteMatchInfos = OWP.PreMatchNotification.InfoSiteMatchInfo.Services;
using NewLeagues = OWP.PreMatchNotification.NewLeague.Services;

namespace OWP.PreMatchNotification;

public static class DependenceInjection
{
    public static void AddPrematchNotificationServices(this IServiceCollection services)
    {
        services.AddTransient<PreMatchNotificationServices.CommandRepository, PreMatchNotificationServices.CommandRepositoryImp>();
        services.AddTransient<PreMatchNotificationServices.MessageFactory, PreMatchNotificationServices.MessageFactoryImp>();
        services.AddTransient<PreMatchNotificationServices.NotifyServices, PreMatchNotificationServices.NotifyServicesImp>();
        services.AddTransient<PreMatchNotificationServices.CacheManipulatorCommon, PreMatchNotificationServices.CacheManipulatorCommonImp>();
        services.AddTransient<PreMatchNotificationServices.DateTimeHelper, PreMatchNotificationServices.DateTimeHelperImp>();

        services.AddTransient<MyAssignment.MyAssignmentGenerator, MyAssignment.MyAssignmentGeneratorImp>();
        services.AddTransient<MyAssignment.MyAssignmentDetector, MyAssignment.MyAssignmentDetectorImp>();
        services.AddTransient<MyAssignment.ChangedValueHdpDetector, MyAssignment.ChangedValueHdpDetectorImp>();
        services.AddTransient<MyAssignment.ChangedValueOuDetector, MyAssignment.ChangedValueOuDetectorImp>();
        services.AddTransient<MyAssignment.ChangedValueHdpTakeOverDetector, MyAssignment.ChangedValueHdpTakeOverDetectorImp>();
        services.AddTransient<MyAssignment.ChangedValueOuTakeOverDetector, MyAssignment.ChangedValueOuTakeOverDetectorImp>();

        services.AddTransient<OtherAssignment.OtherAssignmentGenerator, OtherAssignment.OtherAssignmentGeneratorImp>();
        services.AddTransient<OtherAssignment.OtherAssignmentDetector, OtherAssignment.OtherAssignmentDetectorImp>();
        services.AddTransient<OtherAssignment.ChangedValueHdpDetector, OtherAssignment.ChangedValueHdpDetectorImp>();
        services.AddTransient<OtherAssignment.ChangedValueOuDetector, OtherAssignment.ChangedValueOuDetectorImp>();
        services.AddTransient<OtherAssignment.ChangedValueHdpTakeOverDetector, OtherAssignment.ChangedValueHdpTakeOverDetectorImp>();
        services.AddTransient<OtherAssignment.ChangedValueOuTakeOverDetector, OtherAssignment.ChangedValueOuTakeOverDetectorImp>();

        // LinkedMatch
        services.AddTransient<LinkedMatchs.CacheManipulator, LinkedMatchs.CacheManipulatorImp>();
        services.AddTransient<LinkedMatchs.ChangedLinkedMatchDetector, LinkedMatchs.ChangedLinkedMatchDetectorImp>();
        services.AddTransient<LinkedMatchs.ChangedLiveMatchGenerator, LinkedMatchs.ChangedLiveMatchGeneratorImp>();
        services.AddTransient<LinkedMatchs.ChangedMatchStatusGenerator, LinkedMatchs.ChangedMatchStatusGeneratorImp>();
        services.AddTransient<LinkedMatchs.Repository, LinkedMatchs.RepositoryImp>();

        // InfoSiteMatchInfo
        services.AddTransient<InfoSiteMatchInfos.Repository, InfoSiteMatchInfos.RepositoryImp>();
        services.AddTransient<InfoSiteMatchInfos.ChangedValueGenerator, InfoSiteMatchInfos.ChangedValueGeneratorImp>();

        // NewLeague
        services.AddTransient<NewLeagues.NewLeagueValueGenerator, NewLeagues.NewLeagueValueGeneratorImp>();
    }
}
