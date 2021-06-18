using Autofac;
using EasyNow.Utility.Cache;

namespace EasyNow.Utility
{
    public class UtilityModule:Module
    {
        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EasyCachingLock>().AsImplementedInterfaces().SingleInstance();
        }
    }
}