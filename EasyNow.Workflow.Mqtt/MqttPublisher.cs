using System;
using System.Threading;
using System.Threading.Tasks;
using Elsa;
using Elsa.ActivityResults;
using Elsa.Services;
using Elsa.Attributes;
using Elsa.Services.Models;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Client.Publishing;

namespace EasyNow.Workflow.Mqtt
{
    [Activity(
        Category = "MQTT",
        DisplayName = "发送MQTT消息",
        Description = "发送消息到MQTT",
        Outcomes = new[] { OutcomeNames.Done,"Success","Fail" }
    )]
    public class MqttPublisher:Activity
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

        [ActivityInput(Hint = "消息",Label = "消息",DefaultValue = "Hello", SupportedSyntaxes = new string[] {"Literal","JavaScript", "Liquid"})]
        public string Message { get; set; }

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            var factory = new MqttFactory();
            using var mqttClient = factory.CreateMqttClient();
            var optionsBuilder = new MqttClientOptionsBuilder()
                .WithClientId(this.ClientId)
                .WithTcpServer(this.Server, this.ServerPort)
                .WithCredentials(this.Username, this.Password);
            if (Tls)
            {
                optionsBuilder = optionsBuilder.WithTls();
            }
            
            var options = optionsBuilder
                .WithCleanSession()
                .Build();
            await mqttClient.ConnectAsync(options, context.CancellationToken);

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(this.Topic)
                .WithPayload(this.Message)
                .WithAtLeastOnceQoS()
                .WithRetainFlag()
                .Build();

            var result = await mqttClient.PublishAsync(message, context.CancellationToken);
            if (result.ReasonCode == MqttClientPublishReasonCode.Success)
            {
                return this.Combine(this.Done(), this.Outcome("Success"));
            }

            return this.Combine(this.Done(), this.Outcome("Fail",new
            {
                FailReason=result.ReasonString
            }));
        }
    }
}
