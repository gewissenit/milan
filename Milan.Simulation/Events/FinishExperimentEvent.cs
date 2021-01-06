#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;


namespace Milan.Simulation.Events
{
  public sealed class FinishExperimentEvent : SimulationEvent
  {
    public FinishExperimentEvent(object sender, IExperiment experiment)
      : base(sender, "Finish Experiment")
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
      Experiment.Finish();
    }
  }
}