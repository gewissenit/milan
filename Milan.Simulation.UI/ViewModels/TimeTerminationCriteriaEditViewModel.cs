#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.ComponentModel;
using Emporer.WPF.ViewModels;
using Milan.Simulation.Observers;

namespace Milan.Simulation.UI.ViewModels
{
  public class TimeTerminationCriteriaEditViewModel : EditViewModel
  {
    public TimeTerminationCriteriaEditViewModel(ITimeTerminationCriteria terminationCriteria)
      : base(terminationCriteria, "Time Termination Criteria")
    {
      TerminationCriterion = terminationCriteria;
      TerminationCriterion.PropertyChanged += RaiseInternalPropertyChanges;
    }

    private ITimeTerminationCriteria TerminationCriterion { get; set; }
    
    public TimeSpan Duration
    {
      get
      {
        return TerminationCriterion.Duration;
      }
      set
      {
        TerminationCriterion.Duration = value;
        NotifyOfPropertyChange(()=>Duration);
      }
    }

    public DateTime StopDate
    {
      get { return TerminationCriterion.StopDate; }
      set
      {
        TerminationCriterion.StopDate = value;
        NotifyOfPropertyChange(()=>StopDate);
      }
    }

    private void RaiseInternalPropertyChanges(object sender, PropertyChangedEventArgs e)
    {
      switch (e.PropertyName)
      {
        case "StartDate":
          NotifyOfPropertyChange(()=>StopDate);
          break;
        case "StopDate":
          NotifyOfPropertyChange(()=>StopDate);
          NotifyOfPropertyChange(() => Duration);
          break;
        case "Duration":
          NotifyOfPropertyChange(() => Duration);
          NotifyOfPropertyChange(()=>StopDate);
          break;
      }
    }
  }
}