// <copyright file="UFDateTimeTools.cs" company="Ultra Force Development">
// Ultra Force Library - Copyright (C) 2018 Ultra Force Development
// </copyright>
// <author>Josha Munnik</author>
// <email>josha@ultraforce.com</email>
// <license>
// The MIT License (MIT)
//
// Copyright (C) 2018 Ultra Force Development
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to 
// deal in the Software without restriction, including without limitation the 
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or 
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
// IN THE SOFTWARE.
// </license>

using System;

namespace UltraForce.Library.NetStandard.Tools
{
  /// <summary>
  /// <see cref="DateTime" /> related utility methods.
  /// </summary>
  public static class UFDateTimeTools
  {
    /// <summary>
    /// Gets current time/date in milliseconds, just returns 
    /// GetMilliseconds(DateTime.Now)
    /// </summary>
    /// <returns><see cref="DateTime.Now" /> in milliseconds.</returns>
    public static long GetMilliseconds()
    {
      return GetMilliseconds(DateTime.Now);
    }

    /// <summary>
    /// Get time/date in milliseconds.
    /// </summary>
    /// <returns>aTime.Ticks in milliseconds.</returns>
    /// <param name="aTime">A time/date.</param>
    public static long GetMilliseconds(DateTime aTime)
    {
      return aTime.Ticks / TimeSpan.TicksPerMillisecond;
    }

    /// <summary>
    /// Calculates the difference in months.
    /// </summary>
    /// <param name="aStart">
    /// Starting month
    /// </param>
    /// <param name="anEnd">
    /// Ending month (inclusive)
    /// </param>
    /// <param name="anIgnoreDay">
    /// When <c>true</c> ignore day, else adjust the result if the difference
    /// between days is less then a month.
    /// </param>
    /// <returns>Number of months difference</returns>
    public static int CalcDifferenceMonths(
      DateTime aStart,
      DateTime anEnd,
      bool anIgnoreDay = false
    )
    {
      if (aStart.Month == anEnd.Month)
      {
        return 0;
      }
      int result = anEnd.Month
        + 12 * anEnd.Year
        - aStart.Month
        - 12 * aStart.Year;
      if (!anIgnoreDay)
      {
        if ((aStart < anEnd) && (aStart.Day > anEnd.Day))
        {
          result--;
        }
        else if ((aStart > anEnd) && (aStart.Day < anEnd.Day))
        {
          result++;
        }
      }
      return result;
    }

    /// <summary>
    /// Compares the date part of a <see cref="DateTime"/> type. The method 
    /// checks if the year, month and day are equal.
    /// </summary>
    /// <param name="aDate1">First date to compare</param>
    /// <param name="aDate2">Second date to compare</param>
    /// <returns><c>True</c> if dates are equal.</returns>
    public static bool EqualDate(DateTime aDate1, DateTime aDate2)
    {
      return (aDate1.Year == aDate2.Year) && (aDate1.Month == aDate2.Month) &&
        (aDate1.Day == aDate2.Day);
    }

    /// <summary>
    /// Calculates the number of periods within a date range with a certain
    /// period length in days.
    /// </summary>
    /// <param name="aDays">Number of days in period</param>
    /// <param name="aPeriodStartDate">Start date of first period</param>
    /// <param name="aStartDate">Start date of range</param>
    /// <param name="anEndDate">End date of range</param>
    /// <returns>Number of periods</returns>
    public static int CalcPeriodCountWithDays(
      int aDays,
      DateTime aPeriodStartDate,
      DateTime aStartDate,
      DateTime anEndDate
    )
    {
      long startDay = aStartDate.Subtract(aPeriodStartDate).Ticks / TimeSpan.TicksPerDay;
      long endDay = anEndDate.Subtract(aPeriodStartDate).Ticks / TimeSpan.TicksPerDay;
      if (startDay <= 0)
      {
        startDay = 0;
      }
      else
      {
        long countBefore = startDay / aDays;
        startDay = aDays * countBefore;
      }
      return Math.Max(0, (int)(endDay - startDay) / aDays);
    }

    /// <summary>
    /// Calculates the next date of a period of a certain days.
    /// </summary>
    /// <param name="aDays">Number of days in period</param>
    /// <param name="aPeriodStartDate">Start date of first period</param>
    /// <param name="aDate">Date to get next date on or after</param>
    /// <returns>next date</returns>
    public static DateTime CalcNextPeriodDateWithDays(
      int aDays,
      DateTime aPeriodStartDate,
      DateTime aDate
    )
    {
      long startDay = Math.Max(0, aDate.Subtract(aPeriodStartDate).Ticks / TimeSpan.TicksPerDay);
      int nextDay = (int)Math.Ceiling((double)startDay / aDays);
      return aPeriodStartDate.AddDays(aDays * nextDay);
    }

    /// <summary>
    /// Calculates the nearest date of a period of a certain days before
    /// a certain date.
    /// </summary>
    /// <param name="aDays">Number of days in period</param>
    /// <param name="aPeriodStartDate">Start date of first period</param>
    /// <param name="aDate">Date to get nearest date before</param>
    /// <returns>next date</returns>
    public static DateTime CalcNearestPeriodDateWithDays(
      int aDays,
      DateTime aPeriodStartDate,
      DateTime aDate
    )
    {
      return CalcNextPeriodDateWithDays(aDays, aPeriodStartDate, aDate)
        .AddDays(-aDays);
    }

    /// <summary>
    /// Calculates the number of periods within a date range with a certain
    /// period length in months.
    /// </summary>
    /// <param name="aMonths">Number of months in period</param>
    /// <param name="aPeriodStartDate">Start date of first period</param>
    /// <param name="aStartDate">Start date of range</param>
    /// <param name="anEndDate">End date of range</param>
    /// <returns>Number of periods</returns>
    public static int CalcPeriodCountWithMonths(
      int aMonths,
      DateTime aPeriodStartDate,
      DateTime aStartDate,
      DateTime anEndDate
    )
    {
      // exit if end date becomes before start date
      if (anEndDate.CompareTo(aStartDate) < 0)
      {
        return 0;
      }
      // exit if period starts after end date
      if (anEndDate.CompareTo(aPeriodStartDate) < 0)
      {
        return 0;
      }
      int result = 0;
      for (DateTime date = aPeriodStartDate;
        date.CompareTo(anEndDate) < 0;
        date = date.AddMonths(aMonths)
      )
      {
        if (date.CompareTo(aStartDate) >= 0)
        {
          result++;
        }
      }
      return result;
    }

    /// <summary>
    /// Calculates the next date of a period of a certain months.
    /// </summary>
    /// <param name="aMonths">Number of months in period</param>
    /// <param name="aPeriodStartDate">Start date of first period</param>
    /// <param name="aDate">Date to get next date on or after</param>
    /// <returns>next date</returns>
    public static DateTime CalcNextPeriodDateWithMonths(
      int aMonths,
      DateTime aPeriodStartDate,
      DateTime aDate
    )
    {
      DateTime date = aPeriodStartDate;
      while (date < aDate)
      {
        date = date.AddMonths(aMonths);
      }
      return date;
    }

    /// <summary>
    /// Calculates the nearest date of a period of a certain months before
    /// a certain date.
    /// </summary>
    /// <param name="aMonths">Number of months in period</param>
    /// <param name="aPeriodStartDate">Start date of first period</param>
    /// <param name="aDate">Date to get nearest date before</param>
    /// <returns>next date</returns>
    public static DateTime CalcNearestPeriodDateWithMonths(
      int aMonths,
      DateTime aPeriodStartDate,
      DateTime aDate
    )
    {
      return CalcNextPeriodDateWithMonths(aMonths, aPeriodStartDate, aDate)
        .AddMonths(-aMonths);
    }

    /// <summary>
    /// Calculates the number of periods within a date range with a certain
    /// period length in years.
    /// </summary>
    /// <param name="aYears">Number of years in period</param>
    /// <param name="aPeriodStartDate">Start date of first period</param>
    /// <param name="aStartDate">Start date of range</param>
    /// <param name="anEndDate">End date of range</param>
    /// <returns>Number of periods</returns>
    public static int CalcPeriodCountWithYears(
      int aYears,
      DateTime aPeriodStartDate,
      DateTime aStartDate,
      DateTime anEndDate
    )
    {
      // exit if end date becomes before start date
      if (anEndDate.CompareTo(aStartDate) < 0)
      {
        return 0;
      }
      // exit if period starts after end date
      if (anEndDate.CompareTo(aPeriodStartDate) < 0)
      {
        return 0;
      }
      int result = 0;
      for (DateTime date = aPeriodStartDate;
        date.CompareTo(anEndDate) < 0;
        date = date.AddYears(aYears)
      )
      {
        if (date.CompareTo(aStartDate) >= 0)
        {
          result++;
        }
      }
      return result;
    }

    /// <summary>
    /// Calculates the next date of a period of a certain years.
    /// </summary>
    /// <param name="aYears">Number of years in period</param>
    /// <param name="aPeriodStartDate">Start date of first period</param>
    /// <param name="aDate">Date to get next date on or after</param>
    /// <returns>next date</returns>
    public static DateTime CalcNextPeriodDateWithYears(
      int aYears,
      DateTime aPeriodStartDate,
      DateTime aDate
    )
    {
      DateTime date = aPeriodStartDate;
      while (date < aDate)
      {
        date = date.AddYears(aYears);
      }
      return date;
    }

    /// <summary>
    /// Calculates the nearest date of a period of a certain years before
    /// a certain date.
    /// </summary>
    /// <param name="aYears">Number of years in period</param>
    /// <param name="aPeriodStartDate">Start date of first period</param>
    /// <param name="aDate">Date to get nearest date before</param>
    /// <returns>next date</returns>
    public static DateTime CalcNearestPeriodDateWithYears(
      int aYears,
      DateTime aPeriodStartDate,
      DateTime aDate
    )
    {
      return CalcNextPeriodDateWithYears(aYears, aPeriodStartDate, aDate)
        .AddYears(-aYears);
    }

    /// <summary>
    /// Gets the last day of the month.
    /// </summary>
    /// <param name="aDate">Date to get last day of month for</param>
    /// <returns>last day of the month</returns>
    public static int GetLastDayOfMonth(DateTime aDate)
    {
      DateTime nextMonth = aDate.AddMonths(1);
      DateTime lastDay = nextMonth.AddDays(-nextMonth.Day);
      return lastDay.Day;
    }
    
    /// <summary>
    /// Calculates the age of a person based on the birth date and a reference date.
    /// </summary>
    /// <param name="aBirthDate">Date of birth</param>
    /// <param name="aNow">When null, use the current UTC date and time.</param>
    /// <returns>Age in years</returns>
    public static int CalcAge(DateTime aBirthDate, DateTime? aNow = null)
    {
      aNow ??= DateTime.UtcNow;
      int age = aNow.Value.Year - aBirthDate.Year;
      if (aNow < aBirthDate.AddYears(age))
      {
        age--;
      }
      return age;
    }
  
    /// <summary>
    /// Checks if a year is a leap year.
    /// </summary>
    /// <param name="aYear">Year to check</param>
    /// <returns>True if year is a leap year</returns>
    public static bool IsLeapYear(int aYear)
    {
      return ((aYear % 400) == 0) || (((aYear % 100) != 0) && ((aYear % 4) == 0));
    }
  }
}