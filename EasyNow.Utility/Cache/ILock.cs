using System;

namespace EasyNow.Utility.Cache
{
    public interface ILock
    {
        /// <summary>
        /// 判断是否已锁
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Locked(string key);

        /// <summary>
        /// 指定回调、最大重试次数与间隔时间的锁
        /// </summary>
        /// <param name="key"></param>
        /// <param name="action"></param>
        /// <param name="maxRetries"></param>
        /// <param name="retryInterval"></param>
        /// <param name="expireTimeSpan"></param>
        /// <returns></returns>
        bool Lock(string key, Action action, int? maxRetries = null, int retryInterval = 100,
            TimeSpan? expireTimeSpan = default);

        /// <summary>
        /// 指定最大重试次数与间隔时间的锁
        /// </summary>
        /// <param name="key"></param>
        /// <param name="maxRetries">重试次数,默认一直重试</param>
        /// <param name="retryInterval">重试间隔时间，默认100毫秒</param>
        /// <param name="expireTimeSpan">默认一小时</param>
        bool Lock(string key, int? maxRetries = null, int retryInterval = 100, TimeSpan? expireTimeSpan = default);

        /// <summary>
        /// 解锁
        /// </summary>
        /// <param name="key"></param>
        /// <param name="untilSuccess">直到成功为止</param>
        bool Unlock(string key,bool untilSuccess=true);
    }
}