using System.IO;
using Elsa;
using Microsoft.Extensions.DependencyInjection;

namespace EasyNow.Workflow.Mqtt
{
    public static class ServiceCollectionExtensions
    {
        public static ElsaOptionsBuilder AddMqttActivities(
            this ElsaOptionsBuilder options,
            TextReader? standardIn = null,
            TextWriter? standardOut = null)
        {
            options.AddActivity<MqttPublisher>();
            return options;
        }
    }
}