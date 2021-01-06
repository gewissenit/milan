#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;


namespace Milan.Simulation.Events
{
  public sealed class SimulationEndEvent : SimulationEvent
  {
    public SimulationEndEvent(object sender, IExperiment experiment)
      : base(sender, "SimulationEnd")
    {
      if (experiment == null)
      {
        throw new ArgumentNullException();
      }
      Experiment = experiment;
    }

    private IExperiment Experiment { get; set; }

    public override void Handle()
    {
      Experiment.Scheduler.ScheduleAfter(new FinishExperimentEvent(Experiment.Model, Experiment), this);
    }
  }
}