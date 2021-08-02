using System;
using System.Threading.Tasks;
using EasyCaching.Core;
using JetBrains.Annotations;

namespace EasyNow.Utility.Cache
{
    public class EasyCachingLock:ILock
    {
        private readonly IHybridCachingProvider _hybrid;

        public EasyCachingLock(IHybridCachingProvider hybrid)
        {
            _hybrid = hybrid;
        }

        private string GetKey(string key)
        {
            return $"EasyCachingLock_{key}";
        }

        /// <inheritdoc />
        public bool Locked(string key)
        {
            return _hybrid.Exists(GetKey(key));
        }

        /// <inheritdoc />
        public bool Lock([NotNull]string key, [NotNull]Action action, int? maxRetries = null, int retryInterval = 100,
            TimeSpan? expireTimeSpan = default)
        {
            if (this.Lock(key, maxRetries, retryInterval, expireTimeSpan))
            {
                try
                {
                    action();
                }
                finally
                {
                    this.Unlock(key);
                }

                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public bool Lock([NotNull]string key, int? maxRetries = null, int retryInterval = 100, TimeSpan? expireTimeSpan = default)
        {
            key = GetKey(key);

            var n = 0;
            expireTimeSpan ??= TimeSpan.FromMinutes(5);

            while (!maxRetries.HasValue || n < maxRetries)
            {
                if (this._hybrid.TrySet(key, DateTime.Now, expireTimeSpan.Value))
                {
                    return true;
                }

                Task.Delay(retryInterval).Wait();
                n++;
            }

            return false;
        }

        /// <inheritdoc />
        public bool Unlock(string key, bool untilSuccess = true)
        {
            key = GetKey(key);
            var existKey = this._hybrid.Exists(key);
            if (!existKey)
            {
                return false;
            }

            if (untilSuccess)
            {
                while (existKey)
                {
                    this._hybrid.Remove(key);
                    existKey=this._hybrid.Exists(key);
                    Task.Delay(100).Wait();
                }

                return true;
            }
            this._hybrid.Remove(key);
            return !this._hybrid.Exists(key);
        }
    }
}