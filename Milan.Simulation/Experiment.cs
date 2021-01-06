#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using Milan.JsonStore;
using Milan.Simulation.Events;
using Milan.Simulation.Logging;
using Milan.Simulation.Observers;
using Milan.Simulation.Scheduling;
using Newtonsoft.Json;

namespace Milan.Simulation
{
  [JsonObject(MemberSerialization.OptIn)]
  public class Experiment : DomainEntity, IExperiment
  {
    private readonly BehaviorSubject<float> _progressGenerator = new BehaviorSubject<float>(0.0f);
    private Random _seedGenerator;
    private DateTime _startTime;

    public Experiment(int seed, double settlingTime, IExperimentLogWriterProvider logProvider)
    {
      Seed = seed;
      SettlingTime = settlingTime;
      LogProvider = logProvider;
      RunTime = TimeSpan.Zero;
      RunStatus = RunStatus.NotStarted;
    }

    private CancellationToken CancellationToken { get; set; }

    private BehaviorSubject<float> ProgressGenerator
    {
      get { return _progressGenerator; }
    }

    public IScheduler Scheduler { get; private set; }

    public double CurrentTime
    {
      get { return Scheduler.Clock.CurrentTime; }
    }

    public bool Failed { get; set; }
    public IExperimentLogWriterProvider LogProvider { get; private set; }
    public string DataFolder { get; set; }

    public IModel Model
    {
      get { return Get<IModel>(); }
      set { Set(value); }
    }

    public IObservable<float> Progress
    {
      get { return ProgressGenerator; }
    }

    public RunStatus RunStatus
    {
      get { return Get<RunStatus>(); }
      private set { Set(value); }
    }

    public TimeSpan RunTime { get; private set; }

    public int Seed
    {
      get { return Get<int>(); }
      private set
      {
        Set(value);
        _seedGenerator = new Random(Seed);
      }
    }

    public double SettlingTime
    {
      get { return Get<double>(); }
      private set { Set(value); }
    }

    public event EventHandler<EventArgs> Finished;
    public event EventHandler<EventArgs> Initialized;
    public event EventHandler<EventArgs> Paused;
    public event EventHandler<EventArgs> Reseted;
    public event EventHandler<EventArgs> Resumed;
    public event EventHandler<EventArgs> Started;

    public long AcquireInitializationSeed(string requesterInfo)
    {
      var seed = _seedGenerator.Next();
      if (requesterInfo == null)
      {
        requesterInfo = "unknown";
      }
      return seed;
    }

    public void Finish()
    {
      RunTime = DateTime.Now - _startTime;
      RunStatus = CancellationToken.IsCancellationRequested
                    ? RunStatus.Canceled
                    : RunStatus.Finished;
      Raise(Finished);
    }

    /// <exception cref="InvalidOperationException">Experiment must be running to pause!</exception>
    public void Pause()
    {
      if (RunStatus != RunStatus.Running)
      {
        throw new InvalidOperationException("Experiment must be running to pause!");
      }
      RunStatus = RunStatus.Paused;
      Raise(Paused);
    }

    public void Reset()
    {
      Clear();
      RunStatus = RunStatus.NotStarted;
      Raise(Reseted);
    }

    /// <exception cref="InvalidOperationException">Experiment must be paused to resume!</exception>
    public void Resume()
    {
      if (RunStatus != RunStatus.Paused)
      {
        throw new InvalidOperationException("Experiment must be paused to resume!");
      }
      RunStatus = RunStatus.Running;
      Raise(Resumed);
    }

    /// <param name="ct"></param>
    /// <exception cref="InvalidOperationException">
    ///   <c>InvalidOperationException</c>.
    /// </exception>
    public void Start(CancellationToken ct)
    {
      Scheduler = new Scheduler(new Clock(), new BinaryTimeTable());
      var progressChanges = Model.Observers.OfType<ITerminationCriteria>()
                                 .Select(tc => tc.Progress) // progress of all TCs
                                 .CombineLatest() // latest values
                                 .Select(p => p.Max()) // get the greatest
                                 .Distinct(); // only changed
      progressChanges.Subscribe(ProgressGenerator);

      if (RunStatus != RunStatus.NotStarted)
      {
        throw new InvalidOperationException(String.Format("Experiment must be in state {0} before starting! Actual state: {1}.",
                                                          RunStatus.NotStarted,
                                                          RunStatus));
      }

      _startTime = DateTime.Now;
      CancellationToken = ct;

      Model.Initialize();
      Initialize();
      Raise(Initialized);

      RunStatus = RunStatus.Running;
      Raise(Started);
      Proceed();
    }

    public void Step()
    {
      if (RunStatus != RunStatus.Paused)
      {
        return;
      }

      PerformNextStep();
    }

    public void Proceed()
    {
      while (RunStatus == RunStatus.Running)
      {
        PerformNextStep();
      }
    }

    private void Raise(EventHandler<EventArgs> e)
    {
      e?.Invoke(this, new EventArgs());
    }
    
    private void Initialize()
    {
      if (SettlingTime > 0d)
      {
        Scheduler.Schedule(new SettlingTimeFinishedEvent(this, this), SettlingTime);
      }
      else
      {
        foreach (var observer in Model.Observers)
        {
          observer.IsEnabled = true;
        }
      }

      foreach (var observer in Model.Observers)
      {
        observer.Initialize();
      }
    }

    private void PerformNextStep()
    {
      if (CancellationToken.IsCancellationRequested)
      {
        Finish();
      }

      var oldTime = CurrentTime;

      if (Scheduler.CanProcessNextSchedulable())
      {
        Scheduler.ProcessNextSchedulable();

        if (CurrentTime > oldTime)
        {
          RaisePropertyChanged(() => CurrentTime);
        }
      }
      else
      {
        Finish();
      }
    }

    public void Clear()
    {
      Scheduler = null;
      Model.Reset();
    }
  }
}