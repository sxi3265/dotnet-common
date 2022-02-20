using System;
using System.Threading;
using System.Threading.Tasks;
using EasyNow.Collection;
using EasyNow.File;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using Xunit;

namespace Tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            int a = 1;
            a += 1;

            int? b = null;
            if (b.HasValue)
            {
                Console.WriteLine(b.Value);
            }

            b = b ?? 1;
            b ??= 1;
        }
    }
}
