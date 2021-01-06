#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;

namespace Milan.Simulation
{
  public interface IBatch : IEnumerable<IExperiment>
  {
    TimeSpan BatchRunTime { get; }
    DateTime BatchStartedDate { get; }
    string DataFolder { get; }
    string Id { get; }
    IModel Model { get; }

    IObservable<float> Progress { get; }
    RunStatus RunStatus { get; }

    event EventHandler<ExperimentEventArgs> ExperimentAdded;
    event EventHandler<BatchEventArgs> Finished;
    event EventHandler RunStatusChanged;
    event EventHandler<BatchEventArgs> Started;
    void Add(IExperiment experiment);
    void Cancel();
    void Run();
    void Clear();
  }
}