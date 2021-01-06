#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Reactive.Linq;
using Emporer.WPF.ViewModels;
using ReactiveUI;

namespace Milan.Simulation.UI.ViewModels
{
  public class ExperimentViewModel : ReactiveModelWrapper<IExperiment>
  {
    private readonly ObservableAsPropertyHelper<float> _progress;

    public ExperimentViewModel(IExperiment model)
      : base(model)
    {
      model.Progress.Select(p => (float) Math.Round(p, 1))
           .DistinctUntilChanged()
           .ObserveOnDispatcher()
           .ToProperty(this, x => x.Progress, out _progress);
    }

    public float Progress
    {
      get { return _progress.Value; }
    }

    
  }
}