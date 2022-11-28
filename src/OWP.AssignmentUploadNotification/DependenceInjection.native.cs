using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OWP.AssignmentUploadNotification.Services;

namespace OWP.AssignmentUploadNotification
{
    public static class DependenceInjection
    {
        public static IServiceCollection AddAssignmentUpload(this IServiceCollection services)
        {
            services.AddMediatR(typeof(AssignmentUploadHandler).Assembly);

            services.Add(new ServiceDescriptor(typeof(AssigneeChangedValueFilter), typeof(AssigneeMessageFilterImp), ServiceLifetime.Transient));
            services.Add(new ServiceDescriptor(typeof(CommandRepository), typeof(CommandRepositoryImp), ServiceLifetime.Transient));
            services.Add(new ServiceDescriptor(typeof(CacheManipulator), typeof(CacheManipulatorImp), ServiceLifetime.Transient));
            services.Add(new ServiceDescriptor(typeof(ChangedDetector), typeof(ChangedDetectorImp), ServiceLifetime.Transient));
            services.Add(new ServiceDescriptor(typeof(ChangedMatchChangedValueGenerator), typeof(ChangedMatchChangedValueGeneratorImp), ServiceLifetime.Transient));
            services.Add(new ServiceDescriptor(typeof(ChangedValueAggregator), typeof(ChangedValueAggregatorImp), ServiceLifetime.Transient));
            services.Add(new ServiceDescriptor(typeof(ChangedValueBySnapshot), typeof(ChangedValueBySnapshotImp), ServiceLifetime.Transient));
            services.Add(new ServiceDescriptor(typeof(ChangedValueByCacheAndDatabase), typeof(ChangedValueCacheAndDatabaseImp), ServiceLifetime.Transient));
            services.Add(new ServiceDescriptor(typeof(DeletedMatchChangedValueGenerator), typeof(DeletedMatchChangedValueGeneratorImp), ServiceLifetime.Transient));
            services.Add(new ServiceDescriptor(typeof(InsertedMatchChangedValueGenerator), typeof(InsertedMatchChangedValueGeneratorImp), ServiceLifetime.Transient));
            services.Add(new ServiceDescriptor(typeof(MessageFactory), typeof(MessageFactoryImp), ServiceLifetime.Transient));
            services.Add(new ServiceDescriptor(typeof(NotificationSettingQueryRepository), typeof(NotificationSettingQueryRepositoryImp), ServiceLifetime.Transient));
            services.Add(new ServiceDescriptor(typeof(WatcherChangedValueGenerator), typeof(WatcherChangedValueGeneratorImp), ServiceLifetime.Transient));
            services.Add(new ServiceDescriptor(typeof(ChangedMatchSettingGenerator), typeof(ChangedMatchSettingGeneratorImp), ServiceLifetime.Transient));
            services.Add(new ServiceDescriptor(typeof(ChangedMatchStatusGenerator), typeof(ChangedMatchStatusGeneratorImp), ServiceLifetime.Transient));
            services.Add(new ServiceDescriptor(typeof(ChangedMatchInfoGenerator), typeof(ChangedMatchInfoGeneratorImp), ServiceLifetime.Transient));
            services.Add(new ServiceDescriptor(typeof(ChangedAssignmentGenerator), typeof(ChangedAssignmentGeneratorImp), ServiceLifetime.Transient));
            services.Add(new ServiceDescriptor(typeof(QueryRepository), typeof(QueryRepositoryImp), ServiceLifetime.Transient));

            return services;
        }
    }
}