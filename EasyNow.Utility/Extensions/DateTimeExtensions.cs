using System;

namespace EasyNow.Utility.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime StartTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        public static long GetTimeStamp(this DateTime dateTime)
        {
            return (long)(dateTime - StartTime).TotalSeconds;
        }

        public static long GetMillisecondTimeStamp(this DateTime dateTime)
        {
            return (long)(dateTime - StartTime).TotalMilliseconds;
        }
    }
}