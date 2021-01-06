#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Threading;
using Milan.JsonStore;
using Milan.Simulation.Logging;
using Milan.Simulation.Scheduling;

namespace Milan.Simulation
{
  public interface IExperiment : IDomainEntity
  {
    IScheduler Scheduler { get; }
    double CurrentTime { get; }
    string DataFolder { get; set; }
    bool Failed { get; }
    IExperimentLogWriterProvider LogProvider { get; }
    IModel Model { get; set; }
    IObservable<float> Progress { get; }
    RunStatus RunStatus { get; }
    TimeSpan RunTime { get; }
    int Seed { get; }
    double SettlingTime { get; }
    event EventHandler<EventArgs> Finished;
    event EventHandler<EventArgs> Initialized;
    event EventHandler<EventArgs> Paused;
    event EventHandler<EventArgs> Reseted;
    event EventHandler<EventArgs> Resumed;
    event EventHandler<EventArgs> Started;


    /// <summary>
    ///   Acquires a random number generator initialization seed.
    ///   The returned value should vary for every call to avoid random
    ///   generators that produce identical streams of random numbers.
    ///   The provided <paramref name="requesterInfo">parameter</paramref>
    ///   can be used by the experiment to track provided seeds (e.g. logging, tracing)
    /// </summary>
    /// <param name="requesterInfo">The requester info (name/purpose of the consuming random number generator).</param>
    /// <returns>A seed (number) that can be used to initialize random number generators.</returns>
    long AcquireInitializationSeed(string requesterInfo);

    void Finish();
    void Pause();
    void Proceed();
    void Reset();
    void Resume();
    void Start(CancellationToken ct);
    void Step();
    void Clear();
  }
}