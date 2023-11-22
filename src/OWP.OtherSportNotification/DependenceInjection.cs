using Microsoft.Extensions.DependencyInjection;
using OWP.OtherSportNotification.Services;

namespace OWP.OtherSportNotification;

public static class DependenceInjection
{
    public static IServiceCollection AddOtherSport(this IServiceCollection services)
    {
        services.AddMediatR(x => x.RegisterServicesFromAssemblies(typeof(OtherSportNotificationHandler).Assembly));

        services.AddTransient<OtherSportRepository, OtherSportRepositoryImp>();
        services.AddTransient<CacheManipulator, CacheManipulatorImp>();
        services.AddTransient<ChangedValueByCacheAndDatabase, ChangedValueByCacheAndDatabaseImp>();
        services.AddTransient<ChangedValueBySnapshot, ChangedValueBySnapshotImp>();
        services.AddTransient<AssigneeChangedValueFilter, AssigneeMessageFilterImp>();
        services.AddTransient<ChangedValueAggregator, ChangedValueAggregatorImp>();
        services.AddTransient<CacheManipulator, CacheManipulatorImp>();
        services.AddTransient<ChangedDetector, ChangedDetectorImp>();
        services.AddTransient<ChangedMatchStatusGenerator, ChangedMatchStatusGeneratorImp>();
        services.AddTransient<InsertedMatchChangedValueGenerator, InsertedMatchChangedValueGeneratorImp>();
        services.AddTransient<DeletedMatchChangedValueGenerator, DeletedMatchChangedValueGeneratorImp>();
        services.AddTransient<ChangedMatchChangedValueGenerator, ChangedMatchChangedValueGeneratorImp>();
        services.AddTransient<MessageFactory, MessageFactoryImp>();
        services.AddTransient<ChangedMatchSettingGenerator, ChangedMatchSettingGeneratorImp>();
        services.AddTransient<ChangedMatchInfoGenerator, ChangedMatchInfoGeneratorImp>();
        services.AddTransient<ChangedAssignmentGenerator, ChangedAssignmentGeneratorImp>();
        services.AddTransient<ChangedAssignmentNoteGenerator, ChangedAssignmentNoteGeneratorImp>();
        services.AddTransient<NotificationService, NotificationServiceImp>();
        services.AddTransient<IncludeBBSectionService, IncludeBBSectionServiceImp>();

        return services;
    }
}

