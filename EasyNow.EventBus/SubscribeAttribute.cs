using System;

namespace EasyNow.EventBus
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SubscribeAttribute:Attribute
    {
        public string Name { get; set; }

        public SubscribeAttribute(string name)
        {
            this.Name = name;
        }
    }
}