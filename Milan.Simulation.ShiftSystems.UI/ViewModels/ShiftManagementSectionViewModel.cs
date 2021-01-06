#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using Emporer.WPF;
using Emporer.WPF.Commands;

namespace Milan.Simulation.ShiftSystems.UI.ViewModels
{
  public class ShiftManagementSectionViewModel: Screen
  {
    private readonly RelayCommand _addShiftCommand;
    private readonly RelayCommand _duplicateShiftCommand;
    private readonly RelayCommand _removeShiftCommand;
    private readonly IShiftManagement _shiftManagement;
    private double _currentWidth;
    private TimeSpan _duration;
    private ShiftConfiguration _selectedShift;
    private DayOfWeek _startDay;
    private DateTime _startTime;


    public ShiftManagementSectionViewModel(IShiftManagement shiftManagement)
    {
      Week = new[]
             {
               DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday
             };
      _shiftManagement = shiftManagement;
      ShiftsByDay = new ObservableDictionary<DayOfWeek, ObservableCollection<ShiftViewModel>>();
      DisplayName = "shift plan";
      Week.ForEach(day => ShiftsByDay.Add(day, AddShiftViewModelsForAllShiftsBelongingToThisDay(day)));

      _addShiftCommand = new RelayCommand(a => AddShift(), _ => SelectedShift == null);
      _duplicateShiftCommand = new RelayCommand(a => AddShift(), _ => SelectedShift != null);
      _removeShiftCommand = new RelayCommand(a => RemoveShift(), _ => SelectedShift != null);

      StartDay = DayOfWeek.Monday;
    }


    public ObservableDictionary<DayOfWeek, ObservableCollection<ShiftViewModel>> ShiftsByDay { get; private set; }
    public DayOfWeek[] Week { get; private set; }
    public IEnumerable<DayOfWeek> Days { get; private set; }

    public ICommand AddShiftCommand
    {
      get { return _addShiftCommand; }
    }

    public ICommand DuplicateShiftCommand
    {
      get { return _duplicateShiftCommand; }
    }

    public ICommand RemoveShiftCommand
    {
      get { return _removeShiftCommand; }
    }

    public ShiftConfiguration SelectedShift
    {
      get { return _selectedShift; }
      set
      {
        if ((_selectedShift != value))
        {
          UnselectMatchingShiftVMs(_selectedShift);
          _selectedShift = value;
          SelectMatchingShiftVMs(_selectedShift);

          NotifyOfPropertyChange(() => SelectedShift);

          _addShiftCommand.UpdateCanExecute();
          _duplicateShiftCommand.UpdateCanExecute();
          _removeShiftCommand.UpdateCanExecute();

          if (_selectedShift == null)
          {
            return;
          }

          _startDay = SelectedShift.StartDay;
          _startTime = SelectedShift.StartTime;
          _duration = SelectedShift.Duration;

          // backing fields were set above. manually raising related propchange events
          // circumvents unnecessary setting of all the values in newly selected shift (see the setters of the proeprties) 
          NotifyOfPropertyChange(() => StartDay);
          NotifyOfPropertyChange(() => StartTime);
          NotifyOfPropertyChange(() => Duration);
        }
      }
    }

    public double CurrentWidth
    {
      get { return _currentWidth; }
      set
      {
        if (_currentWidth.Equals(value))
        {
          return;
        }
        _currentWidth = value;
      }
    }

    public DayOfWeek StartDay
    {
      get { return _startDay; }
      set
      {
        if (_startDay == value)
        {
          return;
        }
        _startDay = value;

        if (SelectedShift != null)
        {
          SelectedShift.StartDay = _startDay;
        }

        NotifyOfPropertyChange(() => StartDay);
      }
    }


    public DateTime StartTime
    {
      get { return _startTime; }
      set
      {
        if (_startTime == value)
        {
          return;
        }

        _startTime = value;

        if (SelectedShift != null)
        {
          SelectedShift.StartTime = _startTime;
        }

        NotifyOfPropertyChange(() => StartTime);
      }
    }

    public TimeSpan Duration
    {
      get { return _duration; }
      set
      {
        if (_duration == value)
        {
          return;
        }

        _duration = value;

        if (SelectedShift != null)
        {
          SelectedShift.Duration = value;
        }
        NotifyOfPropertyChange(() => Duration);
      }
    }

    private void AddShift()
    {
      var newShift = new ShiftConfiguration
                     {
                       StartDay = StartDay,
                       StartTime = StartTime,
                       Duration = Duration
                     };
      _shiftManagement.AddShift(newShift);
      AddViewModelsFor(newShift);
      SelectedShift = newShift;
    }

    private void RemoveShift()
    {
      if (SelectedShift == null)
      {
        return;
      }

      RemoveViewModelsFor(SelectedShift);
      _shiftManagement.RemoveShift(SelectedShift);
      SelectedShift = _shiftManagement.Shifts.FirstOrDefault();
    }

    private ObservableCollection<ShiftViewModel> AddShiftViewModelsForAllShiftsBelongingToThisDay(DayOfWeek day)
    {
      var shiftVMs = _shiftManagement.Shifts.Where(s => s.Includes(day))
                                     .Select(s => new ShiftViewModel(s, day))
                                     .OrderBy(s => s.StartDay)
                                     .ThenBy(s => s.StartTime)
                                     .ToList();
      shiftVMs.ForEach(AddChangeListener);
      return new ObservableCollection<ShiftViewModel>(shiftVMs);
    }

    private void AddChangeListener(ShiftViewModel shiftVM)
    {
      shiftVM.PropertyChanged += MoveOnStartDayChanged;
    }

    private void RemoveChangeListener(ShiftViewModel shiftVM)
    {
      shiftVM.PropertyChanged -= MoveOnStartDayChanged;
    }


    private void MoveOnStartDayChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName != "StartDay" &&
          e.PropertyName != "BeginsBeforeVisualizedDay" &&
          e.PropertyName != "EndsAfterVisualizedDay")
      {
        return;
      }
      var targetVM = (ShiftViewModel) sender;
      var targetShift = targetVM.Shift;

      RemoveViewModelsFor(targetShift);
      AddViewModelsFor(targetShift);
      SelectedShift = targetShift;
    }

    private void AddViewModelsFor(ShiftConfiguration targetShift)
    {
      AddShiftViewModelFor(targetShift, targetShift.StartDay);
      if (targetShift.IncludesDayChange())
      {
        AddShiftViewModelFor(targetShift, targetShift.StartDay.NextDay());
      }
    }

    private void AddShiftViewModelFor(ShiftConfiguration targetShift, DayOfWeek visualizedDay)
    {
      var shiftVM = new ShiftViewModel(targetShift, visualizedDay);
      AddChangeListener(shiftVM);

      // reset order
      var shiftsOfTheDay = ShiftsByDay[visualizedDay].Concat(new[]
                                                             {
                                                               shiftVM
                                                             })
                                                     .OrderBy(s => s.StartDay)
                                                     .ThenBy(s => s.StartTime)
                                                     .ToList();
      ShiftsByDay[visualizedDay].Clear();
      ShiftsByDay[visualizedDay].AddRangeUnique(shiftsOfTheDay);
    }

    private void RemoveViewModelsFor(ShiftConfiguration targetShift)
    {
      foreach (var shiftVM in GetViewModelsFor(targetShift)
        .ToArray())
      {
        RemoveShiftFromCurrentDay(shiftVM);
      }
    }

    private IEnumerable<ShiftViewModel> GetViewModelsFor(ShiftConfiguration shift)
    {
      return ShiftsByDay.SelectMany(sbd => sbd.Value)
                        .Where(sVM => sVM.Shift == shift);
    }

    private void RemoveShiftFromCurrentDay(ShiftViewModel target)
    {
      var day = GetCurrentDayOfWeek(target);
      ShiftsByDay[day].Remove(target);
      RemoveChangeListener(target);
    }

    private DayOfWeek GetCurrentDayOfWeek(ShiftViewModel target)
    {
      foreach (var key in ShiftsByDay.Keys.Where(key => ShiftsByDay[key].Any(shiftVM => shiftVM == target)))
      {
        return key;
      }

      throw new InvalidOperationException();
    }

    private void SelectMatchingShiftVMs(ShiftConfiguration selectedShift)
    {
      GetViewModelsFor(selectedShift)
        .ToArray()
        .ForEach(sVM => sVM.IsSelected = true);
    }

    private void UnselectMatchingShiftVMs(ShiftConfiguration selectedShift)
    {
      GetViewModelsFor(selectedShift)
        .ToArray()
        .ForEach(sVM => sVM.IsSelected = false);
    }
  }
}