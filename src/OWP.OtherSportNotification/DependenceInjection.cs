using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OWP.OtherSportNotification.Services;
using System.Reflection;

namespace OWP.OtherSportNotification;

public static class DependenceInjection
{
    public static IServiceCollection AddOtherSport(this IServiceCollection services)
    {
        services.AddTransient<OtherSportRepository, OtherSportRepositoryImp>();
        services.AddMediatR(Assembly.GetExecutingAssembly());
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
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
        return services;
    }
}

