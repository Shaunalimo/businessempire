using System;
using System.Globalization;

namespace BusinessEmpire.Utils
{
    /// <summary>
    /// Formatting helpers for currency and large numbers (mobile-friendly display).
    /// </summary>
    public static class CurrencyFormatter
    {
        private static readonly string[] Suffixes = { "", "K", "M", "B", "T", "Qa" };

        public static string Format(double value, bool includeSign = false)
        {
            string sign = includeSign && value > 0 ? "+" : "";
            if (Math.Abs(value) < 1000d)
                return sign + "$" + value.ToString("N0", CultureInfo.InvariantCulture);

            int suffixIndex = 0;
            double scaled = Math.Abs(value);
            while (scaled >= 1000d && suffixIndex < Suffixes.Length - 1)
            {
                scaled /= 1000d;
                suffixIndex++;
            }

            string formatted = scaled.ToString(scaled >= 100 ? "N0" : "N1", CultureInfo.InvariantCulture);
            string prefix = value < 0 ? "-" : sign;
            return prefix + "$" + formatted + Suffixes[suffixIndex];
        }

        public static string FormatCompact(double value) => Format(value);
    }
}
