using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Services;
using Elsa.Services.Bookmarks;
using Elsa.Services.Models;

namespace EasyNow.Workflow.Mqtt
{
    [Activity(
        Category = "MQTT",
        DisplayName = "MQTT消息订阅",
        Description = "从MQTT订阅消息",
        Outcomes = new[] { OutcomeNames.Done,"Success","Fail" }
    )]
    public class MqttSubscriber : Activity
    {
        [ActivityInput(Hint = "消息主题",Label = "消息主题",DefaultValue = "/workflow/mqtt", SupportedSyntaxes = new string[] {"Literal","JavaScript", "Liquid"})]
        public string Topic { get; set; }

        [ActivityInput(Hint = "服务器地址",Label = "服务器地址", SupportedSyntaxes = new string[] {"Literal","JavaScript", "Liquid"})]
        public string Server { get; set; }

        [ActivityInput(Hint = "服务器端口",Label = "服务器端口",DefaultValue = 1883, SupportedSyntaxes = new string[] {"Literal","JavaScript", "Liquid"})]
        public int ServerPort { get; set; }

        [ActivityInput(Hint = "用户名",Label = "用户名", SupportedSyntaxes = new string[] {"Literal","JavaScript", "Liquid"})]
        public string Username { get; set; }

        [ActivityInput(Hint = "密码",Label = "密码", SupportedSyntaxes = new string[] {"Literal","JavaScript", "Liquid"})]
        public string Password { get; set; }

        [ActivityInput(Hint = "客户端Id",Label = "客户端Id", SupportedSyntaxes = new string[] {"Literal","JavaScript", "Liquid"})]
        public string ClientId { get; set; }

        [ActivityInput(Hint = "加密",Label = "加密",DefaultValue = false, SupportedSyntaxes = new string[] {"Literal","JavaScript", "Liquid"})]
        public bool Tls { get; set; }

        protected override ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {

            return new ValueTask<IActivityExecutionResult>(this.Suspend());
        }
    }

    public class MqttSubscriberBookmark : IBookmark
    {
        public string Topic { get; set; }
        public string Server { get; set; }
        public int ServerPort { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ClientId { get; set; }
        public bool Tls { get; set; }
    }

    public class MqttSubscriberBookmarkProvider : BookmarkProvider<MqttSubscriberBookmark, MqttSubscriber>
    {
        public override async ValueTask<IEnumerable<BookmarkResult>> GetBookmarksAsync(BookmarkProviderContext<MqttSubscriber> context, CancellationToken cancellationToken)
        {
            var topic = await context.ReadActivityPropertyAsync(x => x.Topic, cancellationToken);
            var server = await context.ReadActivityPropertyAsync(x => x.Server, cancellationToken);
            var serverPort = await context.ReadActivityPropertyAsync(x => x.ServerPort, cancellationToken);
            var username = await context.ReadActivityPropertyAsync(x => x.Username, cancellationToken);
            var password = await context.ReadActivityPropertyAsync(x => x.Password, cancellationToken);
            var clientId = await context.ReadActivityPropertyAsync(x => x.ClientId, cancellationToken);
            var tls = await context.ReadActivityPropertyAsync(x => x.Tls, cancellationToken);

            if (server == null)
            {
                return Enumerable.Empty<BookmarkResult>();
            }

            return new[]
            {
                Result(new MqttSubscriberBookmark
                {
                    Topic = topic!,
                    Server = server!,
                    ServerPort = serverPort!,
                    Username = username!,
                    Password = password!,
                    ClientId = clientId!,
                    Tls = tls!,

                })
            };
        }
    }
}