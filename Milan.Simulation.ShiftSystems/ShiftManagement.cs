#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Milan.Simulation.Events;
using Milan.Simulation.ShiftSystems.SimulationEvents;
using Newtonsoft.Json;

namespace Milan.Simulation.ShiftSystems
{
  [JsonObject(MemberSerialization.OptIn)]
  public class ShiftManagement : Entity, IShiftManagement
  {
    private readonly ICollection<ShiftConfiguration> _activeShifts;
    private readonly ICollection<DateTime> _holidays;
    private readonly ICollection<ShiftConfiguration> _shifts;
    private IEnumerable<IShiftPlanAware> _shiftPlanClients;
    private IEnumerable<IWorkingTimeDependent> _workingTimeClients;
    private bool _workingTimeInProgress;

    public ShiftManagement()
    {
      Name = string.Empty;
      _activeShifts = new List<ShiftConfiguration>();
      _holidays = new List<DateTime>();
      _shifts = new List<ShiftConfiguration>();
    }

    #region Holidays

    [JsonProperty]
    public IEnumerable<DateTime> Holidays
    {
      get { return _holidays; }
    }

    public void AddHoliday(DateTime day)
    {
      if (_holidays.All(d => d.Date != day.Date))
      {
        _holidays.Add(day);
      }
    }

    public void RemoveHoliday(DateTime day)
    {
      if (_holidays.Any(d => d.Date == day.Date))
      {
        _holidays.Remove(day);
      }
    }

    #endregion

    #region Shifts

    [JsonProperty]
    public IEnumerable<ShiftConfiguration> Shifts
    {
      get { return _shifts; }
    }

    /// <exception cref="ArgumentException">shift</exception>
    /// <exception cref="ArgumentNullException"><paramref name="shift" /> is <c>null</c>.</exception>
    public void AddShift(ShiftConfiguration shift)
    {
      if (shift == null)
      {
        throw new ArgumentNullException("shift");
      }

      if (_shifts.Contains(shift))
      {
        throw new ArgumentException(string.Format("The same shift was already added ({0})", shift), "shift");
      }

      _shifts.Add(shift);
    }

    /// <exception cref="ArgumentNullException"><paramref name="shift" /> is <c>null</c>.</exception>
    public void RemoveShift(ShiftConfiguration shift)
    {
      if (shift == null)
      {
        throw new ArgumentNullException("shift");
      }

      if (!_shifts.Contains(shift))
      {
        throw new ArgumentException(string.Format("Unknown shift ({0})", shift), "shift");
      }
      _shifts.Remove(shift);
    }

    #endregion

    /// <exception cref="ModelConfigurationException">Shiftmanagement needs a start date entity present in the model.</exception>
    public override void Initialize()
    {
      base.Initialize();

      _workingTimeClients = Model.Entities.OfType<IWorkingTimeDependent>();
      _shiftPlanClients = Model.Entities.OfType<IShiftPlanAware>();

      if (!Shifts.Any())
      {
        StartWorkingTime();
        return;
        // If no shifts are configured, we assume the Shiftmanagement is misconfigured
        // all workingtime dependent entities should nevertheless start to do their work.
        //TODO: this works against the expectations of a developer but propably like a user would expect it
      }

      var experimentStartDate = Model.StartDate;

      var activeShifts = Shifts.Where(shift => shift.Includes(experimentStartDate))
                               .ToArray();

      foreach (var activeShift in activeShifts)
      {
        StartShiftNow(activeShift);
      }

      var inActiveShifts = Shifts.Except(activeShifts);

      foreach (var inActiveShift in inActiveShifts)
      {
        ScheduleNextShift(inActiveShift);
      }
    }

    public override void Reset()
    {
      base.Reset();
      _workingTimeClients = null;
      _shiftPlanClients = null;
      _workingTimeInProgress = false;
      _activeShifts.Clear();
    }

    private void StartShiftNow(ShiftConfiguration shift)
    {
      ScheduleShift(shift, 0);
    }


    private void ScheduleNextShift(ShiftConfiguration shift)
    {
      var currentSimulatedTime = CurrentExperiment.GetCurrentTime();

      var nextSimulatedShiftStartTime = GetNextShiftStartDate(shift, currentSimulatedTime);
      var deltaStart = (nextSimulatedShiftStartTime - currentSimulatedTime).ToSimulationTimeSpan();
      ScheduleShift(shift, deltaStart);
    }


    private void ScheduleShift(ShiftConfiguration shift, double deltaStart)
    {
      var startEvent = new ShiftStarted(this, shift)
                       {
                         OnOccur = StartShift
                       };
      startEvent.Schedule(deltaStart);
    }


    private void StartShift(ISimulationEvent shiftStarted)
    {
      var startEvent = (ShiftStarted) shiftStarted;
      var shift = startEvent.Shift;
      _activeShifts.Add(shift);
      _shiftPlanClients.ForEach(spa => spa.OnShiftStarted(shift));

      ScheduleEndEvent(shift, startEvent);

      if (!_workingTimeInProgress)
      {
        StartWorkingTime();
      }
    }

    private void ScheduleEndEvent(ShiftConfiguration shift, ShiftStarted startEvent)
    {
      var endEvent = new ShiftEnded(this, shift, startEvent)
                     {
                       OnOccur = FinishShift
                     };

      var deltaEnd = shift.Duration.ToSimulationTimeSpan();
      endEvent.Schedule(deltaEnd);
    }


    private void FinishShift(ISimulationEvent shiftEnded)
    {
      var finishedShift = ((ShiftEnded) shiftEnded).Shift;
      _activeShifts.Remove(finishedShift);
      _shiftPlanClients.ForEach(spa => spa.OnShiftEnded(finishedShift));

      if (!_activeShifts.Any() &&
          !AnotherShiftWillStartImmediately())
      {
        StopWorkingTime();
      }

      ScheduleNextShift(finishedShift);
    }

    private bool AnotherShiftWillStartImmediately()
    {
      //TODO: this is strange --> we need two calls here for getting a working test?
      var currentDate = CurrentExperiment.GetCurrentTime();
      var nextStartDate = GetNextShiftStartDate();
      return CurrentExperiment.GetCurrentTime() == GetNextShiftStartDate();
    }


    private DateTime GetNextShiftStartDate()
    {
      var nextShiftStart = new DateTime(Convert.ToInt64(Shifts.Min(s => GetNextShiftStartDate(s)
                                                                          .Ticks)));
      return nextShiftStart;
    }

    private DateTime GetNextShiftStartDate(ShiftConfiguration shift)
    {
      return GetNextShiftStartDate(shift, CurrentExperiment.GetCurrentTime());
    }


    private DateTime GetNextShiftStartDate(ShiftConfiguration shift, DateTime currentSimulatedTime)
    {
      var daysToNextShiftstart = currentSimulatedTime.DayOfWeek.GetDaysTo(shift.StartDay);

      //HACK: if shift start (hh:mm) is before current hh:mm next shift start would be in the past
      //TODO: Improve this code

      if (daysToNextShiftstart == 0 && ((currentSimulatedTime.Hour > shift.StartTime.Hour) ||
          (currentSimulatedTime.Hour == shift.StartTime.Hour && currentSimulatedTime.Minute > shift.StartTime.Minute)))
      {
        daysToNextShiftstart = 7;
      }
      
      var day = currentSimulatedTime + TimeSpan.FromDays(daysToNextShiftstart);
      var nextShiftStart = new DateTime(day.Year, day.Month, day.Day, shift.StartTime.Hour, shift.StartTime.Minute, 0);
      return nextShiftStart;
    }


    private void StartWorkingTime()
    {
      new WorkingTimeStarted(this)
      {
        OnOccur = _ =>
                  {
                    _workingTimeInProgress = true;
                    _workingTimeClients.ForEach(c => c.OnWorkingTimeStarted());
                  }
      }.ScheduleAfterCurrent();
    }


    private void StopWorkingTime()
    {
      new WorkingTimeEnded(this)
      {
        OnOccur = _ =>
                  {
                    _workingTimeInProgress = false;
                    _workingTimeClients.ForEach(c => c.OnWorkingTimeEnded());
                  }
      }.ScheduleAfterCurrent();
    }
  }
}