using Microsoft.Extensions.DependencyInjection;
using OWP.PreMatchNotification.Assignment;
using OWP.PreMatchNotification.LinkedMatch;
using OWP.PreMatchNotification.NewLeague;
using OWP.PreMatchNotification.NewLeague.Services;
using LinkedMatches = OWP.PreMatchNotification.LinkedMatch.Services;
using MyAssignment = OWP.PreMatchNotification.Assignment.Services.MyAssignment;
using NewLeagues = OWP.PreMatchNotification.NewLeague.Services;
using OtherAssignment = OWP.PreMatchNotification.Assignment.Services.OtherAssignment;
using PreMatchNotificationServices = OWP.PreMatchNotification.Shared.Services;

namespace OWP.PreMatchNotification;

public static class DependenceInjection
{
    public static void AddPrematchNotificationServices(this IServiceCollection services)
    {
        services.AddMediatR(x => x.RegisterServicesFromAssemblies(typeof(MathInfoNotifyHandler).Assembly));
        services.AddTransient<PreMatchNotificationServices.CommandRepository, PreMatchNotificationServices.CommandRepositoryImp>();
        services.AddTransient<PreMatchNotificationServices.MessageFactory, PreMatchNotificationServices.MessageFactoryImp>();
        services.AddTransient<PreMatchNotificationServices.CacheManipulatorCommon, PreMatchNotificationServices.CacheManipulatorCommonImp>();
        services.AddTransient<PreMatchNotificationServices.DateTimeHelper, PreMatchNotificationServices.DateTimeHelperImp>();
        services.AddTransient<PreMatchNotificationServices.NotificationService, PreMatchNotificationServices.NotificationServiceImp>();


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
        services.AddMediatR(x => x.RegisterServicesFromAssemblies(typeof(LinkedMatchNotifyHandler).Assembly));
        services.AddTransient<LinkedMatches.CacheManipulator, LinkedMatches.CacheManipulatorImp>();
        services.AddTransient<LinkedMatches.ChangedLinkedMatchDetector, LinkedMatches.ChangedLinkedMatchDetectorImp>();
        services.AddTransient<LinkedMatches.ChangedLiveMatchGenerator, LinkedMatches.ChangedLiveMatchGeneratorImp>();
        services.AddTransient<LinkedMatches.ChangedMatchStatusGenerator, LinkedMatches.ChangedMatchStatusGeneratorImp>();
        services.AddTransient<LinkedMatches.Repository, LinkedMatches.RepositoryImp>();

        // InfoSiteMatchInfo
        services.AddMediatR(x => x.RegisterServicesFromAssemblies(typeof(MathInfoNotifyHandler).Assembly));

        // NewLeague
        services.AddMediatR(x => x.RegisterServicesFromAssemblies(typeof(NewLeagueNotifyHandler).Assembly));
        services.AddTransient<NewLeagues.NewLeagueValueGenerator, NewLeagues.NewLeagueValueGeneratorImp>();

        services.AddTransient<NightShiftNotificationService, NightShiftNotificationServiceImp>();
        services.AddTransient<DayShiftNotificationService, DayShiftNotificationServiceImp>();
        services.AddTransient<NotificationServiceCommon, NotificationServiceCommonImp>();
    }
}
