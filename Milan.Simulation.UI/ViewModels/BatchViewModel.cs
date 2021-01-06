#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows.Input;
using Emporer.WPF.Commands;
using Emporer.WPF.ViewModels;
using ReactiveUI;

namespace Milan.Simulation.UI.ViewModels
{
  public class BatchViewModel : ReactiveModelWrapper<IBatch>
  {
    private readonly RelayCommand _cancelBatchCommand;
    private readonly ReactiveList<ExperimentViewModel> _experiments;
    private readonly ObservableAsPropertyHelper<float> _progress;

    public BatchViewModel(IBatch model)
      : base(model)
    {
      Description = string.Format("{0} runs, starting on {1}", model.Count(), model.Model.StartDate);

      _cancelBatchCommand = new RelayCommand(CancelBatch, CanCancelBatch);
      var initialExperiments = model.Select(CreateViewModel);
      _experiments = new ReactiveList<ExperimentViewModel>(initialExperiments);

      Observable.FromEventPattern<EventHandler<ExperimentEventArgs>, ExperimentEventArgs>(h => model.ExperimentAdded += h,
                                                                                          h => model.ExperimentAdded -= h)
                .Select(e => e.EventArgs.Experiment)
                .Select(CreateViewModel)
                .Subscribe(vm => Experiments.Add(vm));

      model.Progress.ObserveOnDispatcher().ToProperty(this, x => x.Progress, out _progress);

      //Model.RunStatusChanged += UpdateRunStatus;
      Observable.FromEventPattern<EventHandler, EventArgs>(h => model.RunStatusChanged += h, h => model.RunStatusChanged -= h)
                .ObserveOn(DispatcherScheduler.Current)
                .Subscribe(_ => UpdateRunStatus(null, null));
    }


    public ICommand CancelBatchCommand
    {
      get { return _cancelBatchCommand; }
    }

    public string Description { get; private set; }

    public ReactiveList<ExperimentViewModel> Experiments
    {
      get { return _experiments; }
    }

    public float Progress
    {
      get { return _progress.Value; }
    }

    private bool CanCancelBatch(object obj)
    {
      return Model.RunStatus == RunStatus.Running || Model.RunStatus == RunStatus.Paused;
    }

    private void CancelBatch(object obj)
    {
      Model.Cancel();
    }

    private ExperimentViewModel CreateViewModel(IExperiment model)
    {
      return new ExperimentViewModel(model);
    }

    private void UpdateRunStatus(object sender, EventArgs e)
    {
      _cancelBatchCommand.UpdateCanExecute();
    }
  }
}