#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Linq;

namespace Milan.Simulation.ShiftSystems
{
  /// <summary>
  ///   Utilities and extension methods for the shift management domain.
  /// </summary>
  public static class Utils
  {
    /// <summary>
    ///   Checks whether the shift includes the given date.
    /// </summary>
    /// <param name="shift">The shift.</param>
    /// <param name="date">The date.</param>
    /// <returns></returns>
    /// <remarks>
    ///   the given date is taken as an exact point in time, not a time span.
    ///   This means the date '2012-12-24' is interpreted as '2012-12-24 0:00:0000'.
    /// </remarks>
    public static bool Includes(this ShiftConfiguration shift, DateTime date)
    {
      var daysOfShift = shift.IncludesDayChange()
                          ? new[]
                            {
                              shift.StartDay, TheDayAfter(shift.StartDay)
                            }
                          : new[]
                            {
                              shift.StartDay
                            };

      if (!daysOfShift.Any(d => d == date.DayOfWeek))
      {
        return false; // the date is on the wrong day of the week
      }

      //apply shift settings to date to calculate simulated shift start/end 

      //if the dates day is the second day of the shift, we have to substract one day to get the real shift start day
      var realYear = date.Year;
      var realMonth = date.Month;
      var realDayOfMonth = daysOfShift[0] == date.DayOfWeek
                             ? date.Day
                             : date.Day - 1;

      if (realDayOfMonth < 1)
      {
        var tmp = new DateTime(date.Year, date.Month, 1, shift.StartTime.Hour, shift.StartTime.Minute, 0);
        realDayOfMonth = (tmp - TimeSpan.FromDays(1)).Day;

        realMonth--;
        if (realMonth < 1)
        {
          realMonth = 12;
          realYear--;
        }
      }

      var realShiftStart = new DateTime(realYear, realMonth, realDayOfMonth, shift.StartTime.Hour, shift.StartTime.Minute, 0);
      var realShiftEnd = realShiftStart + shift.Duration;

      return realShiftStart <= date && date <= realShiftEnd;
    }


    /// <summary>
    ///   Checks whether the shift includes (at least partially) the given day.
    /// </summary>
    /// <param name="shift">The shift.</param>
    /// <param name="day">The day.</param>
    /// <returns></returns>
    public static bool Includes(this ShiftConfiguration shift, DayOfWeek day)
    {
      var daysOfShift = shift.IncludesDayChange()
                          ? new[]
                            {
                              shift.StartDay, TheDayAfter(shift.StartDay)
                            }
                          : new[]
                            {
                              shift.StartDay
                            };
      return daysOfShift.Any(d => d == day);
    }


    /// <summary>
    ///   Checks whether the shift includes a change of day.
    /// </summary>
    /// <param name="shift">The shift.</param>
    /// <returns></returns>
    public static bool IncludesDayChange(this ShiftConfiguration shift)
    {
      return TimeSpan.FromMinutes(shift.StartTime.Hour * 60 + shift.StartTime.Minute + shift.Duration.TotalMinutes) > TimeSpan.FromDays(1);
    }


    /// <summary>
    ///   Gets the nexts day.
    /// </summary>
    /// <param name="day">A day.</param>
    /// <returns></returns>
    public static DayOfWeek NextDay(this DayOfWeek day)
    {
      return TheDayAfter(day);
    }

    /// <summary>
    ///   Gets the previous day.
    /// </summary>
    /// <param name="day">A day.</param>
    /// <returns></returns>
    public static DayOfWeek PreviousDay(this DayOfWeek day)
    {
      return TheDayBefore(day);
    }


    /// <summary>
    ///   Gets the day (of week) after the given day.
    /// </summary>
    /// <param name="day">A day.</param>
    /// <returns>The next day.</returns>
    public static DayOfWeek TheDayAfter(DayOfWeek day)
    {
      if (day == DayOfWeek.Saturday)
      {
        return DayOfWeek.Sunday;
      }
      return ++day;
    }


    /// <summary>
    ///   Gets the day (of week) before the given day.
    /// </summary>
    /// <param name="day">A day.</param>
    /// <returns>The previous day.</returns>
    public static DayOfWeek TheDayBefore(DayOfWeek day)
    {
      if (day == DayOfWeek.Sunday)
      {
        return DayOfWeek.Saturday;
      }
      return --day;
    }


    public static DateTime GetCurrentTime(this IExperiment experiment)
    {
      var currentInternalTime = experiment.CurrentTime;
      var startTime = experiment.Model.StartDate;
      return currentInternalTime.ToRealDate(startTime);
    }


    public static int GetDaysTo(this DayOfWeek first, DayOfWeek second)
    {
      var days = 0; //same day;
      if (first > second)
      {
        days = 7 + (int) second - (int) first; //day of this week
      }
      else if (first < second)
      {
        days = second - first; //day of the next week
      }
      return days;
    }
  }
}