using System;

namespace MasterSpriggans.Utils
{
    /// <summary>
    ///     Utility class for converting UTC time to other time zones.
    /// </summary>
    public static class TimeConverter
    {
        /// <summary>
        ///     Converts a UTC DatTime object to Easter Standard Time.
        /// </summary>
        /// <param name="utc">
        ///     The UTC DateTime object to convert.
        /// </param>
        /// <returns>
        ///     A DateTime object represented in Eastern Standard Time.
        /// </returns>
        public static DateTime UtcToEst(DateTime utc) =>
            TimeZoneInfo.ConvertTimeFromUtc(utc, TimeZoneInfo.FindSystemTimeZoneById(@"Eastern Standard Time"));

        /// <summary>
        ///     Converts a UTC DatTime object to Central Standard Time.
        /// </summary>
        /// <param name="utc">
        ///     The UTC DateTime object to convert.
        /// </param>
        /// <returns>
        ///     A DateTime object represented in Central Standard Time.
        /// </returns>
        public static DateTime UtcToCst(DateTime utc) =>
            TimeZoneInfo.ConvertTimeFromUtc(utc, TimeZoneInfo.FindSystemTimeZoneById(@"Central Standard Time"));

        /// <summary>
        ///     Converts a UTC DatTime object to Mountain Standard Time.
        /// </summary>
        /// <param name="utc">
        ///     The UTC DateTime object to convert.
        /// </param>
        /// <returns>
        ///     A DateTime object represented in Mountain Standard Time.
        /// </returns>
        public static DateTime UtcToMst(DateTime utc) =>
            TimeZoneInfo.ConvertTimeFromUtc(utc, TimeZoneInfo.FindSystemTimeZoneById(@"Mountain Standard Time"));

        /// <summary>
        ///     Converts a UTC DatTime object to Pacific Standard Time.
        /// </summary>
        /// <param name="utc">
        ///     The UTC DateTime object to convert.
        /// </param>
        /// <returns>
        ///     A DateTime object represented in Pacific Standard Time.
        /// </returns>
        public static DateTime UtcToPst(DateTime utc) =>
            TimeZoneInfo.ConvertTimeFromUtc(utc, TimeZoneInfo.FindSystemTimeZoneById(@"Pacific Standard Time"));

        /// <summary>
        ///     Converts a UTC DateTime objec to New Zealand Standard Time.
        /// </summary>
        /// <param name="utc">
        ///     The UTC DateTime object to convert.
        /// </param>
        /// <returns>
        ///     A DateTime object represented in New Zealand Standard Time.
        /// </returns>
        public static DateTime UtcToNzst(DateTime utc) =>
            TimeZoneInfo.ConvertTimeFromUtc(utc, TimeZoneInfo.FindSystemTimeZoneById(@"New Zealand Standard Time"));
    }
}