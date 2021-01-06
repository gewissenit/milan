#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace Milan.Simulation
{
  public class Batch : IBatch
  {
    private readonly IList<IExperiment> _experiments;
    private readonly BehaviorSubject<float> _progressGenerator;
    private CancellationTokenSource _cancellationTokensource;
    private RunStatus _runStatus;

    public Batch(string id, IModel model, string dataFolder)
    {
      Id = id;
      DataFolder = dataFolder;
      Model = model;
      _experiments = new List<IExperiment>();
      _progressGenerator = new BehaviorSubject<float>(0.0f);
      RunStatus = RunStatus.NotStarted;
      BatchRunTime = TimeSpan.Zero;
      ExperimentsExecutionTime = TimeSpan.Zero;
    }

    public TimeSpan ExperimentsExecutionTime { get; private set; }
    public TimeSpan BatchRunTime { get; private set; }
    public DateTime BatchStartedDate { get; private set; }
    public string DataFolder { get; private set; }
    public string Id { get; private set; }
    public IModel Model { get; private set; }

    public IObservable<float> Progress
    {
      get { return _progressGenerator; }
    }

    public RunStatus RunStatus
    {
      get { return _runStatus; }
      private set
      {
        _runStatus = value;
        RaiseRunStatusChanged();
      }
    }

    public event EventHandler<ExperimentEventArgs> ExperimentAdded;
    public event EventHandler<BatchEventArgs> Finished;
    public event EventHandler RunStatusChanged;
    public event EventHandler<BatchEventArgs> Started;

    public void Add(IExperiment experiment)
    {
      _experiments.Add(experiment);
      RaiseExperimentAdded(experiment);
    }

    public void Cancel()
    {
      if (RunStatus != RunStatus.Running &&
          RunStatus != RunStatus.Paused)
      {
        return;
      }

      _cancellationTokensource.Cancel();
    }

    public IEnumerator<IExperiment> GetEnumerator()
    {
      return _experiments.Where(exp => !exp.Failed)
                         .GetEnumerator();
    }

    public void Run()
    {
      if (RunStatus != RunStatus.NotStarted)
      {
        throw new InvalidOperationException("A batch can not be started twice.");
      }

      _cancellationTokensource = new CancellationTokenSource();
      RunStatus = RunStatus.Running;
      BatchStartedDate = DateTime.Now;

      // setup observation
      var experiments = _experiments.ToObservable();

      var progressChanges = _experiments.Select(experiment => experiment.Progress)
                                        .CombineLatest()
                                        .Select(l => l.Sum() / _experiments.Count);
      progressChanges.Subscribe(_progressGenerator);

      RaiseStarted();

      var ct = _cancellationTokensource.Token;
      Parallel.ForEach(_experiments,
                       e =>
                       {
                         try
                         {
                           e.Start(ct);
                         }
                         catch (Exception)
                         {
                           RunStatus = RunStatus.Error;
                           throw;
                         }
                       });
      FinishBatch();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public void Clear()
    {
      _experiments.ForEach(e => e.Clear());
    }


    public override string ToString()
    {
      return string.Format("Batch {0:G}. ({1} runs)", BatchStartedDate, this.Count());
    }

    private void FinishBatch()
    {
      BatchRunTime = DateTime.Now - BatchStartedDate;
      RunStatus = _cancellationTokensource.IsCancellationRequested
                    ? RunStatus.Canceled
                    : RunStatus.Finished;
      RaiseFinished();
    }

    private void RaiseExperimentAdded(IExperiment experiment)
    {
      ExperimentAdded?.Invoke(this, new ExperimentEventArgs(experiment));
    }

    private void RaiseFinished()
    {
      Finished?.Invoke(this, new BatchEventArgs(this));
    }

    private void RaiseRunStatusChanged()
    {
      RunStatusChanged?.Invoke(this, new EventArgs());
    }

    private void RaiseStarted()
    {
      Started?.Invoke(this, new BatchEventArgs(this));
    }
  }
}