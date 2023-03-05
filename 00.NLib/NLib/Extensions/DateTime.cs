#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2014-10-15
=================
- NLib Extension Methods for DateTime and TimeSpan added.
    - Add method: ToDateString with supports set time part and culture.
    - Add method: ToDateTime which convert string to DateTime instance.

======================================================================================================================
Update 2013-01-01
=================
- NLib Extension Methods for DateTime and TimeSpan added.
  - Add new Extenstion methods for work with DateTime and TimeSpan.
    - Add methods: January to December for change int to DateTime.
    - Add methods: Days, Hours, Minutes and Seconds for change int to TimeSpan.
    - Add methods: Ago, AgoSince, FromNow and From to change TimeSpan to DateTime.
    - Add method: ElapseTime.
    - Add methods: To, DownTo, StepTo for build DateTime Range.
    - Add method: ToDateString for format DateTime to string (invariant).

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

#endregion

namespace NLib
{
    #region DateTimeExteionMethods

    /// <summary>
    /// The Extension methods for DateTime and Timespan.
    /// </summary>
    public static class DateTimeExtensionMethods
    {
        #region DateTime From Int32

        /// <summary>
        /// Build DateTime from specificed day of month for January at specificed year.
        /// Example : DateTime d = 11.January(2008);
        /// </summary>
        /// <param name="i">The day of month.</param>
        /// <param name="year">The year.</param>
        /// <returns>
        /// Returns DateTime for specificed The day of month.
        /// If specificed day of month is less than zero the first day of month will returns
        /// If specificed day of month is over than max day in month the last day of month will returns
        /// </returns>
        public static DateTime January(this int i, int year)
        {
            int month = 1;
            if (i > 31)
                return new DateTime(year, month, 31);
            else if (i <= 0)
                return new DateTime(year, month, 1);

            return new DateTime(year, month, i);
        }
        /// <summary>
        /// Build DateTime from specificed day of month for February at specificed year.
        /// Example : DateTime d = 11.February(2008);
        /// </summary>
        /// <param name="i">The day of month.</param>
        /// <param name="year">The year.</param>
        /// <returns>
        /// Returns DateTime for specificed The day of month.
        /// If specificed day of month is less than zero the first day of month will returns
        /// If specificed day of month is over than max day in month the last day of month will returns
        /// </returns>
        public static DateTime February(this int i, int year)
        {
            int month = 2;
            int maxDayInMonth = DateTime.DaysInMonth(year, month);
            if (i > maxDayInMonth)
                return new DateTime(year, month, maxDayInMonth);
            else if (i <= 0)
                return new DateTime(year, month, 1);

            return new DateTime(year, month, i);
        }
        /// <summary>
        /// Build DateTime from specificed day of month for March at specificed year.
        /// Example : DateTime d = 11.March(2008);
        /// </summary>
        /// <param name="i">The day of month.</param>
        /// <param name="year">The year.</param>
        /// <returns>
        /// Returns DateTime for specificed The day of month.
        /// If specificed day of month is less than zero the first day of month will returns
        /// If specificed day of month is over than max day in month the last day of month will returns
        /// </returns>
        public static DateTime March(this int i, int year)
        {
            int month = 3;
            if (i > 31)
                return new DateTime(year, month, 31);
            else if (i <= 0)
                return new DateTime(year, month, 1);

            return new DateTime(year, month, i);
        }
        /// <summary>
        /// Build DateTime from specificed day of month for April at specificed year.
        /// Example : DateTime d = 11.April(2008);
        /// </summary>
        /// <param name="i">The day of month.</param>
        /// <param name="year">The year.</param>
        /// <returns>
        /// Returns DateTime for specificed The day of month.
        /// If specificed day of month is less than zero the first day of month will returns
        /// If specificed day of month is over than max day in month the last day of month will returns
        /// </returns>
        public static DateTime April(this int i, int year)
        {
            int month = 4;
            if (i > 30)
                return new DateTime(year, month, 30);
            else if (i <= 0)
                return new DateTime(year, month, 1);

            return new DateTime(year, month, i);
        }
        /// <summary>
        /// Build DateTime from specificed day of month for May at specificed year.
        /// Example : DateTime d = 11.May(2008);
        /// </summary>
        /// <param name="i">The day of month.</param>
        /// <param name="year">The year.</param>
        /// <returns>
        /// Returns DateTime for specificed The day of month.
        /// If specificed day of month is less than zero the first day of month will returns
        /// If specificed day of month is over than max day in month the last day of month will returns
        /// </returns>
        public static DateTime May(this int i, int year)
        {
            int month = 5;
            if (i > 31)
                return new DateTime(year, month, 31);
            else if (i <= 0)
                return new DateTime(year, month, 1);

            return new DateTime(year, month, i);
        }
        /// <summary>
        /// Build DateTime from specificed day of month for June at specificed year.
        /// Example : DateTime d = 11.June(2008);
        /// </summary>
        /// <param name="i">The day of month.</param>
        /// <param name="year">The year.</param>
        /// <returns>
        /// Returns DateTime for specificed The day of month.
        /// If specificed day of month is less than zero the first day of month will returns
        /// If specificed day of month is over than max day in month the last day of month will returns
        /// </returns>
        public static DateTime June(this int i, int year)
        {
            int month = 6;
            if (i > 30)
                return new DateTime(year, month, 30);
            else if (i <= 0)
                return new DateTime(year, month, 1);

            return new DateTime(year, month, i);
        }
        /// <summary>
        /// Build DateTime from specificed day of month for July at specificed year.
        /// Example : DateTime d = 11.July(2008);
        /// </summary>
        /// <param name="i">The day of month.</param>
        /// <param name="year">The year.</param>
        /// <returns>
        /// Returns DateTime for specificed The day of month.
        /// If specificed day of month is less than zero the first day of month will returns
        /// If specificed day of month is over than max day in month the last day of month will returns
        /// </returns>
        public static DateTime July(this int i, int year)
        {
            int month = 7;
            if (i > 31)
                return new DateTime(year, month, 31);
            else if (i <= 0)
                return new DateTime(year, month, 1);

            return new DateTime(year, month, i);
        }
        /// <summary>
        /// Build DateTime from specificed day of month for August at specificed year.
        /// Example : DateTime d = 11.August(2008);
        /// </summary>
        /// <param name="i">The day of month.</param>
        /// <param name="year">The year.</param>
        /// <returns>
        /// Returns DateTime for specificed The day of month.
        /// If specificed day of month is less than zero the first day of month will returns
        /// If specificed day of month is over than max day in month the last day of month will returns
        /// </returns>
        public static DateTime August(this int i, int year)
        {
            int month = 8;
            if (i > 31)
                return new DateTime(year, month, 31);
            else if (i <= 0)
                return new DateTime(year, month, 1);

            return new DateTime(year, month, i);
        }
        /// <summary>
        /// Build DateTime from specificed day of month for September at specificed year.
        /// Example : DateTime d = 11.September(2008);
        /// </summary>
        /// <param name="i">The day of month.</param>
        /// <param name="year">The year.</param>
        /// <returns>
        /// Returns DateTime for specificed The day of month.
        /// If specificed day of month is less than zero the first day of month will returns
        /// If specificed day of month is over than max day in month the last day of month will returns
        /// </returns>
        public static DateTime September(this int i, int year)
        {
            int month = 9;
            if (i > 30)
                return new DateTime(year, month, 30);
            else if (i <= 0)
                return new DateTime(year, month, 1);

            return new DateTime(year, month, i);
        }
        /// <summary>
        /// Build DateTime from specificed day of month for October at specificed year.
        /// Example : DateTime d = 11.October(2008);
        /// </summary>
        /// <param name="i">The day of month.</param>
        /// <param name="year">The year.</param>
        /// <returns>
        /// Returns DateTime for specificed The day of month.
        /// If specificed day of month is less than zero the first day of month will returns
        /// If specificed day of month is over than max day in month the last day of month will returns
        /// </returns>
        public static DateTime October(this int i, int year)
        {
            int month = 10;
            if (i > 31)
                return new DateTime(year, month, 31);
            else if (i <= 0)
                return new DateTime(year, month, 1);

            return new DateTime(year, month, i);
        }
        /// <summary>
        /// Build DateTime from specificed day of month for November at specificed year.
        /// Example : DateTime d = 11.November(2008);
        /// </summary>
        /// <param name="i">The day of month.</param>
        /// <param name="year">The year.</param>
        /// <returns>
        /// Returns DateTime for specificed The day of month.
        /// If specificed day of month is less than zero the first day of month will returns
        /// If specificed day of month is over than max day in month the last day of month will returns
        /// </returns>
        public static DateTime November(this int i, int year)
        {
            int month = 11;
            if (i > 30)
                return new DateTime(year, month, 30);
            else if (i <= 0)
                return new DateTime(year, month, 1);

            return new DateTime(year, month, i);
        }
        /// <summary>
        /// Build DateTime from specificed day of month for December at specificed year.
        /// Example : DateTime d = 11.December(2008);
        /// </summary>
        /// <param name="i">The day of month.</param>
        /// <param name="year">The year.</param>
        /// <returns>
        /// Returns DateTime for specificed The day of month.
        /// If specificed day of month is less than zero the first day of month will returns
        /// If specificed day of month is over than max day in month the last day of month will returns
        /// </returns>
        public static DateTime December(this int i, int year)
        {
            int month = 12;
            if (i > 31)
                return new DateTime(year, month, 31);
            else if (i <= 0)
                return new DateTime(year, month, 1);

            return new DateTime(year, month, i);
        }

        #endregion

        #region TimeSpan from Int32

        /// <summary>
        /// Gets the TimeSpan instance for specificed days.
        /// Usage : TimeSpan ts = 20.Days();
        /// </summary>
        /// <param name="i">The number of days</param>
        /// <returns>Returns TimeSpan instance.</returns>
        public static TimeSpan Days(this int i)
        {
            return new TimeSpan(i, 0, 0, 0);
        }
        /// <summary>
        /// Gets the TimeSpan instance for specificed hours.
        /// Usage : TimeSpan ts = 20.Hours();
        /// </summary>
        /// <param name="i">The number of hours</param>
        /// <returns>Returns TimeSpan instance.</returns>
        public static TimeSpan Hours(this int i)
        {
            return new TimeSpan(i, 0, 0);
        }
        /// <summary>
        /// Gets the TimeSpan instance for specificed minutes.
        /// Usage : TimeSpan ts = 20.Minutes();
        /// </summary>
        /// <param name="i">The number of minutes</param>
        /// <returns>Returns TimeSpan instance.</returns>
        public static TimeSpan Minutes(this int i)
        {
            return new TimeSpan(0, i, 0);
        }
        /// <summary>
        /// Gets the TimeSpan instance for specificed seconds.
        /// Usage : TimeSpan ts = 20.Seconds();
        /// </summary>
        /// <param name="i">The number of seconds</param>
        /// <returns>Returns TimeSpan instance.</returns>
        public static TimeSpan Seconds(this int i)
        {
            return new TimeSpan(0, 0, i);
        }

        #endregion

        #region DateTime from TimeSpan

        /// <summary>
        /// Gets the DateTime by specificed time criteria.
        /// For Example when need to get DateTime from 20 minutes ago we can write something 
        /// like : 20.Minutes().Ago();
        /// </summary>
        /// <param name="ts">The TimeSpan interval that used for substract from DateTime.Now.</param>
        /// <returns>Returns DateTime instance.</returns>
        public static DateTime Ago(this TimeSpan ts)
        {
            return DateTime.Now - ts;
        }
        /// <summary>
        /// Gets the DateTime by specificed time criteria.
        /// For Example when need to find 15 days before 1 january 2009 
        /// we can write something like : 15.Days().AgoSince(1.January(2009));
        /// </summary>
        /// <param name="ts">The TimeSpan interval that used for substract from the 'dt' parameter.</param>
        /// <param name="dt">The Starting DateTime.</param>
        /// <returns>Returns DateTime instance.</returns>
        public static DateTime AgoSince(this TimeSpan ts, DateTime dt)
        {
            return dt - ts;
        }
        /// <summary>
        /// Gets the DateTime by specificed time criteria.
        /// For Example when need to gets the Date that is 8 hour from now we can write something
        /// like : 8.Hours().FromNow();
        /// </summary>
        /// <param name="ts">The TimeSpan interval that used for add to DateTime.Now.</param>
        /// <returns>Returns DateTime instance.</returns>
        public static DateTime FromNow(this TimeSpan ts)
        {
            return DateTime.Now + ts;
        }
        /// <summary>
        /// Gets the DateTime by specificed time criteria.
        /// For Example when need to find 3 days after 5 december 2008 we can write something
        /// like : 3.Days().From(5.December(2008));
        /// </summary>
        /// <param name="ts">The TimeSpan interval that used for add to the 'dt' parameter.</param>
        /// <param name="dt">The Starting DateTime.</param>
        /// <returns>Returns DateTime instance.</returns>
        public static DateTime From(this TimeSpan ts, DateTime dt)
        {
            return dt + ts;
        }

        #endregion

        #region Elapse

        /// <summary>
        /// Find Elapse time,
        /// </summary>
        /// <param name="dt">The starting datetime.</param>
        /// <returns>Returns TimeSpan instance.</returns>
        public static TimeSpan ElapseTime(this DateTime dt)
        {
            return DateTime.Now - dt;
        }

        #endregion

        #region DateTime Range

        /// <summary>
        /// Gets All DateTime from start date to end date.
        /// i.e. From First of year to ToDay we can write something 
        /// like : 1.January(2008).To(DateTime.Now);
        /// </summary>
        /// <param name="from">The Starting Range.</param>
        /// <param name="to">The Ending Range.</param>
        /// <returns>Returns IEnumerable of DateTime insance.</returns>
        public static IEnumerable<DateTime> To(this DateTime from,
            DateTime to)
        {
            for (DateTime dt = from; dt <= to; dt = dt.AddDays(1.0))
                yield return dt;
        }
        /// <summary>
        /// Gets All DateTime from start date to end date.
        /// i.e. From End of year to Next 7 days we can write something 
        /// like : 31.December(2008).DownTo(7.Days().FromNow());
        /// </summary>
        /// <param name="from">The Starting Range.</param>
        /// <param name="to">The Ending Range.</param>
        /// <returns>Returns IEnumerable of DateTime insance.</returns>
        public static IEnumerable<DateTime> DownTo(this DateTime from,
            DateTime to)
        {
            for (DateTime dt = from; dt >= to; dt = dt.AddDays(-1.0))
                yield return dt;
        }
        /// <summary>
        /// Gets All DateTime from start date to end date.
        /// i.e. From Start of year to end of year every 3 days we can write something 
        /// like : 1.January(2008).StepTo(31.December(2008), 3.Days());
        /// </summary>
        /// <param name="from">The Starting Range.</param>
        /// <param name="to">The Ending Range.</param>
        /// <param name="step">The step.</param>
        /// <returns>Returns IEnumerable of DateTime insance.</returns>
        public static IEnumerable<DateTime> StepTo(this DateTime from,
            DateTime to, TimeSpan step)
        {
            if (step.Ticks > 0L)
            {
                for (DateTime dt = from; dt <= to; dt = dt.Add(step))
                    yield return dt;
            }
            else if (step.Ticks < 0L)
            {
                for (DateTime dt = from; dt >= to; dt = dt.Add(step))
                    yield return dt;
            }
            else throw new ArgumentException("step cannot be zero");
        }

        #endregion

        #region ToDateString

        /// <summary>
        /// To DateTime String Invariant.
        /// </summary>
        /// <param name="value">The DateTime instance.</param>
        /// <param name="format">
        /// The custom date format.
        /// Default format is yyyy.MM.dd.HH.mm.ss.ffff
        /// </param>
        /// <returns>
        /// Returns string that represnets the DateTime that render in specificed format.
        /// </returns>
        public static string ToDateTimeString(this DateTime value,
            string format = "yyyy.MM.dd.HH.mm.ss.ffff")
        {
            return value.ToString(format,
                System.Globalization.DateTimeFormatInfo.InvariantInfo);
        }
        /// <summary>
        /// To Date String Invariant. The output seperator always be '/'.
        /// </summary>
        /// <param name="dateValue">The DateTime instance.</param>
        /// <param name="timeValue">Optional time value. Default is 00:00:00.</param>
        /// <param name="dateformat">Optional date format. Default is yyyy/MM/dd.</param>
        /// <returns>Returns string that format the to match specificed format.</returns>
        public static string ToDateString(this DateTime dateValue,
            string timeValue = "00:00:00", string dateformat = "yyyy/MM/dd")
        {
            try
            {
                string sDate = dateValue.Date.ToString(dateformat,
                    DateTimeFormatInfo.InvariantInfo);
                return sDate.Replace("-", "/") + " " + timeValue;
            }
            catch (Exception)
            {
                string sDate = DateTime.Today.Date.ToString(dateformat,
                    DateTimeFormatInfo.InvariantInfo);
                return sDate.Replace("-", "/") + " " + timeValue;
            }
        }

        #endregion

        #region ToDateTime

        /// <summary>
        /// Convert string to datetime in specificed culture code.
        /// </summary>
        /// <param name="dateTimeString">The string to convert.</param>
        /// <param name="format">Optional format.</param>
        /// <param name="cultureCode">Optional culture code.</param>
        /// <returns>Returns datetime instance.</returns>
        public static DateTime ToDateTime(this string dateTimeString,
            string format = "dd/MM/yyyy", string cultureCode = "en-US")
        {
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureCode);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(cultureCode);
            }
            catch
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            }

            if (string.IsNullOrWhiteSpace(dateTimeString))
                return DateTime.Today;
            try
            {
                return DateTime.ParseExact(dateTimeString, format,
                        DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None);
            }
            catch (Exception)
            {
                return DateTime.Today;
            }
        }

        #endregion
    }

    #endregion
}
