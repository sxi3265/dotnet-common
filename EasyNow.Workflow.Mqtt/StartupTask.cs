using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Services;
using Elsa.Services.Bookmarks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using Elsa;
using Elsa.Events;
using Elsa.Services.Dispatch;
using Elsa.Services.Triggers;
using MediatR;
using NetBox.Extensions;

namespace EasyNow.Workflow.Mqtt
{
    public class StartupTask:IStartupTask
    {
        private readonly IBookmarkFinder _bookmarkFinder;
        private readonly IWorkflowInstanceDispatcher _workflowInstanceDispatcher;

        private const string? TenantId = default;

        public StartupTask(IBookmarkFinder bookmarkFinder, IWorkflowInstanceDispatcher workflowInstanceDispatcher)
        {
            _bookmarkFinder = bookmarkFinder;
            _workflowInstanceDispatcher = workflowInstanceDispatcher;
        }


        public async Task ExecuteAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var triggers = await _bookmarkFinder.FindBookmarksAsync<MqttSubscriber>(cancellationToken: cancellationToken);

            foreach (var result in triggers)
            {
                var trigger = (MqttSubscriberBookmark) result.Bookmark;
                await _workflowInstanceDispatcher.DispatchAsync(
                    new ExecuteWorkflowInstanceRequest(result.WorkflowInstanceId, result.ActivityId), cancellationToken);
            }


            //var factory = new MqttFactory();
            //var mqttClient = factory.CreateMqttClient();
            //var optionsBuilder = new MqttClientOptionsBuilder()
            //    .WithClientId("testclient1")
            //    .WithTcpServer("103.127.126.204", 30005)
            //    .WithCredentials("admin", "tDVpZs4fEEwabekn");
            
            //var options = optionsBuilder
            //    .WithCleanSession()
            //    .Build();

            //mqttClient.UseConnectedHandler(async e =>
            //{
            //    await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("/workflow/mqtt").Build());
            //});

            //mqttClient.UseApplicationMessageReceivedHandler(e =>
            //{
            //    Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
            //    Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
            //    Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
            //    Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
            //    Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
            //    Console.WriteLine();

            //    Task.Run(() => mqttClient.PublishAsync("hello/world"));
            //});

            //await mqttClient.ConnectAsync(options, CancellationToken.None);


        }

        public int Order => 3000;
    }

    public static class RabbitmqManager
    {
        public static ConcurrentDictionary<string,ConcurrentBag<TriggerFinderResult>> Triggers = new ConcurrentDictionary<string,ConcurrentBag<TriggerFinderResult>>();

        public static ConcurrentDictionary<string, IMqttClient> MqttClients =
            new ConcurrentDictionary<string, IMqttClient>();

        public static ConcurrentDictionary<IMqttClient, ConcurrentBag<string>> MqttClientTopicsDic =
            new ConcurrentDictionary<IMqttClient, ConcurrentBag<string>>();


        public static void Add(TriggerFinderResult result)
        {
            if (!result.WorkflowBlueprint.IsLatest || !result.WorkflowBlueprint.IsPublished)
            {
                return;
            }

            var bookmark = (result.Bookmark as MqttSubscriberBookmark)!;
            var key = $"{bookmark.Server}_{bookmark.ServerPort}_{bookmark.Username}_{bookmark.Password}_{bookmark.ClientId}_{bookmark.Tls}";
            var list = Triggers.GetOrAdd(key, k => new ConcurrentBag<TriggerFinderResult>());
            list.Add(result);
            ConfigClient();
        }

        public static void ConfigClient()
        {
            foreach (var trigger in Triggers)
            {
                var mqttClient = MqttClients.GetOrAdd(trigger.Key, k =>
                {
                    var bookmark = (trigger.Value.First().Bookmark as MqttSubscriberBookmark)!;
                    var factory = new MqttFactory();
                    var mqttClient = factory.CreateMqttClient();
                    var optionsBuilder = new MqttClientOptionsBuilder()
                        .WithClientId(bookmark.ClientId)
                        .WithTcpServer(bookmark.Server, bookmark.ServerPort)
                        .WithCredentials(bookmark.Username, bookmark.Password);

                    var options = optionsBuilder
                        .WithCleanSession()
                        .Build();

                    mqttClient.UseApplicationMessageReceivedHandler(e =>
                    {
                        var topic = e.ApplicationMessage.Topic;
                        if (Triggers.TryGetValue(k, out var triggers))
                        {
                            triggers.Select(e => (e.Bookmark as MqttSubscriberBookmark).Topic == topic).ForEach(
                                async result =>
                                {
                                    //await _workflowInstanceDispatcher.DispatchAsync(
                                    //    new ExecuteWorkflowInstanceRequest(result.WorkflowInstanceId, result.ActivityId), cancellationToken);
                                });
                        }
                    });

                    mqttClient.ConnectAsync(options, CancellationToken.None).Wait();
                    
                    return mqttClient;
                });
                var topics = MqttClientTopicsDic.GetOrAdd(mqttClient, k => new ConcurrentBag<string>());
                foreach (var result in trigger.Value)
                {
                    var bookmark = (result.Bookmark as MqttSubscriberBookmark)!;
                    if (!topics.Contains(bookmark.Topic))
                    {
                        mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(bookmark.Topic).Build()).Wait();
                        topics.Add(bookmark.Topic);
                    }
                }
                
            }
        }
    }

    public class MqttSubscriberTrigger : INotificationHandler<TriggerIndexingFinished>
    {
        private readonly ITriggerFinder _triggerFinder;

        public MqttSubscriberTrigger(ITriggerFinder triggerFinder)
        {
            _triggerFinder = triggerFinder;
        }

        public async Task Handle(TriggerIndexingFinished notification, CancellationToken cancellationToken)
        {
            var triggers = await _triggerFinder.FindTriggersAsync<MqttSubscriber>(null,cancellationToken:cancellationToken);
            //throw new NotImplementedException();
            foreach (var result in triggers)
            {
                RabbitmqManager.Add(result);
            }
        }
    }
}