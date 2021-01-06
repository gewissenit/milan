#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.ComponentModel;
using Caliburn.Micro;

namespace Milan.Simulation.ShiftSystems.UI.ViewModels
{
  public class ShiftViewModel : PropertyChangedBase
  {
    private readonly ShiftConfiguration _shift;
    private readonly DayOfWeek _visualizedDay;
    private bool _beginsBeforeVisualizedDay;
    private double _currentWidth;
    private double _durationOffset;
    private bool _endsAfterVisualizedDay;
    private bool _isSelected;
    private double _startOffset;

    public ShiftViewModel(ShiftConfiguration shift, DayOfWeek visualizedDay)
    {
      _shift = shift;
      _visualizedDay = visualizedDay;
      _shift.PropertyChanged += RaisePropertyChangeAccordingToWrappedShift;

      _currentWidth = 0d;

      UpdateBeginsBeforeVisualizedDay();
      UpdateEndsAfterVisualizedDay();
      UpdateStartOffset();
      UpdateDurationOffset();
    }


    public ShiftConfiguration Shift
    {
      get { return _shift; }
    }


    public bool IsSelected
    {
      get { return _isSelected; }
      set
      {
        if (_isSelected == value)
        {
          return;
        }
        _isSelected = value;
        NotifyOfPropertyChange(() => IsSelected);
      }
    }


    public double CurrentWidth
    {
      get { return _currentWidth; }
      set
      {
        if (_currentWidth.Equals(value)) //||value<240d)
        {
          //_CurrentWidth = 240d;
          return;
        }
        _currentWidth = value;
        UpdateStartOffset();
        UpdateDurationOffset();
      }
    }

    private double UnitsPerHour
    {
      get { return CurrentWidth / 24; }
    }

    private double UnitsPerMinute
    {
      get { return UnitsPerHour / 60; }
    }


    public DayOfWeek StartDay
    {
      get { return _shift.StartDay; }
      set
      {
        if ((_shift.StartDay == value))
        {
          return;
        }
        _shift.StartDay = value;
        UpdateBeginsBeforeVisualizedDay();
      }
    }


    public DayOfWeek VisualizedDay
    {
      get { return _visualizedDay; }
    }

    /// <exception cref="ArgumentException">Hour has to be a number between 0 and 23</exception>
    public DateTime StartTime
    {
      get { return _shift.StartTime; }
      set
      {
        if ((_shift.StartTime == value))
        {
          return;
        }

        _shift.StartTime = value;
        UpdateStartOffset();
      }
    }
    
    /// <exception cref="ArgumentException">Value has to be between 0 and 23</exception>
    public TimeSpan Duration
    {
      get { return _shift.Duration; }
      set
      {
        if ((_shift.Duration == value))
        {
          return;
        }
        _shift.Duration = value;
        UpdateDurationOffset();
        UpdateEndsAfterVisualizedDay();
      }
    }

    public double StartOffset
    {
      get { return _startOffset; }
      set
      {
        if (_startOffset.Equals(value))
        {
          return;
        }
        _startOffset = value;
        NotifyOfPropertyChange(() => StartOffset);
      }
    }

    public double DurationOffset
    {
      get { return _durationOffset; }
      set
      {
        if (_durationOffset.Equals(value))
        {
          return;
        }
        _durationOffset = value;
        NotifyOfPropertyChange(() => DurationOffset);
      }
    }


    public bool EndsAfterVisualizedDay
    {
      get { return _endsAfterVisualizedDay; }
      set
      {
        if ((_endsAfterVisualizedDay != value))
        {
          _endsAfterVisualizedDay = value;
          NotifyOfPropertyChange(() => EndsAfterVisualizedDay);
        }
      }
    }


    public bool BeginsBeforeVisualizedDay
    {
      get { return _beginsBeforeVisualizedDay; }
      set
      {
        if ((_beginsBeforeVisualizedDay != value))
        {
          _beginsBeforeVisualizedDay = value;

          NotifyOfPropertyChange(() => BeginsBeforeVisualizedDay);
        }
      }
    }

    private void RaisePropertyChangeAccordingToWrappedShift(object sender, PropertyChangedEventArgs e)
    {
      switch (e.PropertyName)
      {
        case "StartTime":
          NotifyOfPropertyChange(() => StartTime);
          UpdateStartOffset();
          break;
        case "Duration":
          NotifyOfPropertyChange(() => Duration);
          UpdateDurationOffset();
          break;
        default:
          break;
      }
    }

    private void UpdateStartOffset()
    {
      if (StartDay == Utils.TheDayBefore(VisualizedDay))
      {
        StartOffset = 0d;
      }
      else
      {
        StartOffset = StartTime.Hour * UnitsPerHour + StartTime.Minute * UnitsPerMinute;
      }

      UpdateDurationOffset();
    }


    private void UpdateDurationOffset()
    {
      if (StartDay == Utils.TheDayBefore(VisualizedDay))
      {
        var yesterdaysStartOffset = StartTime.Hour * UnitsPerHour + StartTime.Minute * UnitsPerMinute;
        var maxWidth = 24 * UnitsPerHour - yesterdaysStartOffset;
        var fullWidth = Duration.Hours * UnitsPerHour + Duration.Minutes * UnitsPerMinute;
        var width = fullWidth > maxWidth
                      ? maxWidth
                      : fullWidth;
        DurationOffset = fullWidth - width;
      }
      else
      {
        var maxWidth = 24 * UnitsPerHour - StartOffset;
        var fullWidth = Duration.Hours * UnitsPerHour + Duration.Minutes * UnitsPerMinute;
        var width = fullWidth > maxWidth
                      ? maxWidth
                      : fullWidth;
        DurationOffset = width;
      }

      UpdateEndsAfterVisualizedDay();
    }

    private void UpdateBeginsBeforeVisualizedDay()
    {
      BeginsBeforeVisualizedDay = StartDay == Utils.TheDayBefore(VisualizedDay);
    }


    private void UpdateEndsAfterVisualizedDay()
    {
      EndsAfterVisualizedDay = !BeginsBeforeVisualizedDay && ShiftSpansDayChange();
    }

    private bool ShiftSpansDayChange()
    {
      if (StartTime.Hour + Duration.Hours > 24)
      {
        return true;
      }

      if ((StartTime.Hour + Duration.Hours == 23) &&
          (StartTime.Minute + Duration.Minutes > 59))
      {
        return true;
      }

      return false;
    }

    public override string ToString()
    {
      return string.Format("{0} {1}:{2} ({3})", StartDay, Duration.Hours, Duration.Minutes, VisualizedDay);
    }
  }
}