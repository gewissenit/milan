#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Milan.JsonStore;
using Milan.Simulation.Events;
using Milan.Simulation.Logging;
using Milan.Simulation.Scheduling;
using Newtonsoft.Json;

namespace Milan.Simulation.Observers
{
  /// <summary>
  ///   Base class for entity related experiment observers. Extending this class simplifies the creation of observers for
  ///   scheduler events.
  /// </summary>
  public abstract class SchedulerObserver : DomainEntity, ISimulationObserver
  {
    protected ILogFileWriter _logger;

    protected IScheduler Scheduler
    {
      get { return (CurrentExperiment).Scheduler; }
    }

    protected IExperiment CurrentExperiment { get; private set; }

    [JsonProperty]
    public bool IsEnabled
    {
      get { return Get<bool>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public virtual IModel Model
    {
      get { return Get<IModel>(); }
      set { Set(value); }
    }

    public string Name { get; protected set; }
    
    public void Configure(IExperiment cfg)
    {
      CurrentExperiment = cfg;
      CurrentExperiment.Started += OnSimulationStart;
    }

    /// <summary>
    ///   Finalizes the observer. This is called at the end of all experiments that are conducted
    ///   in a <see cref="IBatch">batch</see>, <b>not</b> at the end of the experiment istself(!).
    /// </summary>
    public virtual void Flush()
    {
    }

    public void Initialize()
    {
      if (CurrentExperiment == null)
      {
        throw new InvalidOperationException("This should not occur!");
      }
      if (!IsEnabled)
      {
        return;
      }

      _logger = CurrentExperiment.LogProvider.GetLogger(CurrentExperiment);
      Scheduler.BeforeSchedulableHandling += OnBeforeEventOccurred;
      Scheduler.SchedulableHandled += OnSchedulerSchedulableHandled;
      Scheduler.CollectionChanged += OnSchedulerCollectionChanged;

      AdditionalInitialization();
    }

    public virtual void Reset()
    {
      _logger = null;
    }

    public override string ToString()
    {
      return string.Format("Observer ({0})", Id);
    }

    /// <summary>
    ///   Does additional initialization.
    ///   This is called before the experiment starts.
    /// </summary>
    /// <remarks>
    ///   Overwrite to add your own initialization. Calling base.AnalyzeRemovedEvents is not necessary.
    /// </remarks>
    protected virtual void AdditionalInitialization()
    {
    }


    /// <summary>
    ///   Called when an event has occurred.
    /// </summary>
    /// <param name="e">The e.</param>
    /// <remarks>
    ///   Overwrite to add your analysis. Calling base.AnalyzeOccurredEvent is not necessary.
    /// </remarks>
    protected virtual void OnEventOccurred(ISimulationEvent e)
    {
    }

    protected virtual void OnBeforeEventOccurred(ISimulationEvent e)
    {
    }

    /// <summary>
    ///   Called when events are added.
    /// </summary>
    /// <param name="addedEvents">The added events.</param>
    /// <remarks>
    ///   Overwrite to add your analysis. Calling base.AnalyzeAddedEvents is not necessary.
    /// </remarks>
    protected virtual void OnEventsAdded(IEnumerable<ISimulationEvent> addedEvents)
    {
    }


    /// <summary>
    ///   Called when events are removed.
    /// </summary>
    /// <param name="removedEvents">The removed events.</param>
    /// <remarks>
    ///   Overwrite to add your analysis. Calling base.AnalyzeRemovedEvents is not necessary.
    /// </remarks>
    protected virtual void OnEventsRemoved(IEnumerable<ISimulationEvent> removedEvents)
    {
    }


    /// <summary>
    ///   Called when the observed experiment has finished.
    /// </summary>
    protected virtual void OnSimulationEnd(ISimulationEvent e)
    {
    }

    /// <summary>
    ///   Called when the observed experiment has started.
    /// </summary>
    protected virtual void OnSimulationStart()
    {
    }

    private void OnSchedulerCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.Action == NotifyCollectionChangedAction.Add)
      {
        OnEventsAdded(e.NewItems.OfType<ISimulationEvent>());
      }
      else if (e.Action == NotifyCollectionChangedAction.Remove)
      {
        OnEventsRemoved(e.OldItems.OfType<ISimulationEvent>());
      }
      // other actions are ignored atm
    }


    private void OnBeforeEventOccurred(object sender, SchedulerEventArgs e)
    {
      OnBeforeEventOccurred(e.Schedulable);
    }

    private void OnSchedulerSchedulableHandled(object s, SchedulerEventArgs e)
    {
      OnEventOccurred(e.Schedulable);

      if (e.Schedulable is FinishExperimentEvent)
      {
        OnSimulationEnd(e.Schedulable);
      }
    }

    private void OnSimulationStart(object sender, EventArgs e)
    {
      CurrentExperiment.Started -= OnSimulationStart;
      OnSimulationStart();
    }
  }
}