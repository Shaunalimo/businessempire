using System;
using UnityEngine;

namespace BusinessEmpire.Utils
{
    /// <summary>
    /// Converts real-world timestamps to in-game day calculations.
    /// </summary>
    public static class TimeHelper
    {
        public static long UnixNow => DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        public static double HoursSince(long unixTimestamp)
        {
            if (unixTimestamp <= 0) return 0d;
            return (UnixNow - unixTimestamp) / 3600d;
        }

        public static int DaysSince(long unixTimestamp)
        {
            if (unixTimestamp <= 0) return 0;
            return (int)(HoursSince(unixTimestamp) / 24d);
        }

        public static bool IsSameUtcDay(long timestampA, long timestampB)
        {
            var a = DateTimeOffset.FromUnixTimeSeconds(timestampA).UtcDateTime.Date;
            var b = DateTimeOffset.FromUnixTimeSeconds(timestampB).UtcDateTime.Date;
            return a == b;
        }

        public static bool IsNewUtcDaySince(long lastTimestamp)
        {
            if (lastTimestamp <= 0) return true;
            return !IsSameUtcDay(lastTimestamp, UnixNow);
        }
    }
}
