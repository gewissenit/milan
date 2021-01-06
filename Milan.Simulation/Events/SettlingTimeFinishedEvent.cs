#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Linq;

namespace Milan.Simulation.Events
{
  public sealed class SettlingTimeFinishedEvent : SimulationEvent
  {
    private const string EventName = "SettlingTimeFinished";
    private readonly IExperiment _experiment;

    public SettlingTimeFinishedEvent(object sender, IExperiment experiment)
      : base(sender, EventName)
    {
      _experiment = experiment;
    }

    public override void Handle()
    {
      foreach (var observer in _experiment.Model.Observers.Where(o => !o.IsEnabled))
      {
        observer.IsEnabled = true;
        observer.Initialize();
      }
      base.Handle();
    }
  }
}