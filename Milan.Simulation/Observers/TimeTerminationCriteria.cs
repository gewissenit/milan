#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.ComponentModel;
using System.Reactive.Subjects;

using Milan.Simulation.Events;
using Newtonsoft.Json;

namespace Milan.Simulation.Observers
{
  [JsonObject(MemberSerialization.OptIn)]
  //todo: refactor and move visual things to view model
  public class TimeTerminationCriteria : SchedulerObserver, ITimeTerminationCriteria
  {
    private readonly BehaviorSubject<float> _progressGenerator = new BehaviorSubject<float>(0.0f);

    public TimeTerminationCriteria()
    {
      Name = "Terminate after certain amount of time has passed.";
      Duration = TimeSpan.FromDays(7);
    }
    
    public DateTime StartDate
    {
      get
      {
        if (Model == null)
        {
          return DateTime.Now;
        }
        return Model.StartDate;
      }
    }

    [JsonProperty]
    public TimeSpan Duration
    {
      get { return Get<TimeSpan>(); }
      set
      {
        var oldvalue = Duration;
        if (oldvalue == value)
        {
          return;
        }
        Set(value);
        RaisePropertyChanged(() => StopDate);
      }
    }

    public override IModel Model
    {
      get { return base.Model; }
      set
      {
        if (Model != null)
        {
          Model.PropertyChanged -= ChangeStartDateIfNecessary;
        }
        base.Model = value;
        if (value != null)
        {
          value.PropertyChanged += ChangeStartDateIfNecessary;
        }
      }
    }

    public IObservable<float> Progress
    {
      get { return _progressGenerator; }
    }

    public DateTime StopDate
    {
      get { return StartDate + Duration; }
      set
      {
        var newDuration = TimeSpan.Zero;
        if (value <= StartDate)
        {
          //HACK: if result will be negative, TS.Zero is used.
          //throw new ArgumentException("The stop date has to be greater then start date.");
        }
        else
        {
          newDuration = value - StartDate;
        }

        if (Duration == newDuration)
        {
          return;
        }
        Duration = newDuration;
      }
    }
    
    protected override void OnEventOccurred(ISimulationEvent e)
    {
      base.OnEventOccurred(e);
      _progressGenerator.OnNext(CalculateProgress());
    }

    protected override void OnSimulationStart()
    {
      var simulationEndEvent = new SimulationEndEvent(null, CurrentExperiment);
      CurrentExperiment.Scheduler.Schedule(simulationEndEvent, Duration.ToSimulationTimeSpan());
    }

    private float CalculateProgress()
    {
      var duration = (StopDate - StartDate).Ticks;
      var elapsed = (CurrentExperiment.CurrentTime.ToRealDate(StartDate) - StartDate).Ticks;
      var result = (float) elapsed / duration * 100;
      return result;
    }

    private void ChangeStartDateIfNecessary(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName != "StartDate")
      {
        return;
      }
      RaisePropertyChanged(() => StartDate);
    }
  }
}