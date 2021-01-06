#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;

namespace Milan.Simulation
{
  public class ExperimentEventArgs : EventArgs
  {
    public ExperimentEventArgs(IExperiment experiment)
    {
      Experiment = experiment;
    }

    public IExperiment Experiment { get; private set; }
  }
}