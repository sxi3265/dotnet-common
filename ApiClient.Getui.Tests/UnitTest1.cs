using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EasyNow.ApiClient.Getui;
using EasyNow.Utility.Extensions;
using Newtonsoft.Json;
using Refit;
using Xunit;

namespace ApiClient.Getui.Tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            var getuiApi = RestService.For<IGetuiApi>("https://restapi.getui.com/v2/xxxxx",new RefitSettings
            {
                ContentSerializer = new NewtonsoftJsonContentSerializer(new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                })
            });

            var authResult =await getuiApi.Auth(new AuthReq
            {
                AppKey = "xxx",
                MasterSecret = "xxxxx",
                Timestamp = DateTime.UtcNow.GetMillisecondTimeStamp().ToString()
            });
            var a = await getuiApi.PushSingle(authResult.Data.Token, new PushReq
            {
                RequestId = Guid.NewGuid().ToString("N"),
                Audience = new CidAudience
                {
                    Cid = new[] {"xxxx"},
                },
                PushMessage = new PushMessage
                {
                    Transmission = new
                    {
                        title="aaaaaa",
                        content="测试123123",
                        payload="aaaaa"
                    }.ToJson()
                },
                PushChannel = new PushChannel
                {
                    Android = new ChannelAndroid
                    {
                        Ups = new Ups
                        {
                            //Transmission = new
                            //{
                            //    title="bbbb",
                            //    content="测试cccc",
                            //    payload="aaaaa"
                            //}.ToJson(),
                            Notification = new UpsNotification
                            {
                                Title = "厂商标题",
                                Body = "厂商内容",
                                ClickType = "payload",
                                Payload = new
                                {
                                    title="bbbb",
                                    content="测试cccc",
                                    payload="aaaaa"
                                }.ToJson(),
                            },
                            Options=new Dictionary<string,Dictionary<string,object>>
                            {
                                {"HW",new Dictionary<string, object>
                                {
                                    {"badgeAddNum",1},
                                    {"/message/android/urgency","HIGH"},
                                    {"/message/android/category","PLAY_VOICE"}
                                }}
                            }
                        }
                    }
                }
            });
        }
    }
}
