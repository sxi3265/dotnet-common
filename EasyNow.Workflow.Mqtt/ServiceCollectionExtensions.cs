using System.IO;
using Elsa;
using Elsa.Events;
using Elsa.Runtime;
using Microsoft.Extensions.DependencyInjection;

namespace EasyNow.Workflow.Mqtt
{
    public static class ServiceCollectionExtensions
    {
        public static ElsaOptionsBuilder AddMqttActivities(
            this ElsaOptionsBuilder options)
        {
            options.AddActivity<MqttPublisher>().AddActivity<MqttSubscriber>();
            options.Services.AddNotificationHandler<TriggerIndexingFinished,MqttSubscriberTrigger>().AddStartupTask<StartupTask>().AddBookmarkProvider<MqttSubscriberBookmarkProvider>();
            return options;
        }
    }
}