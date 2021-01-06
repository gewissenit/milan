using System;
using System.Collections.Generic;
using Milan.JsonStore;
using Milan.Simulation.Observers;
using Newtonsoft.Json;

namespace Milan.Simulation
{
  [JsonObject(MemberSerialization.OptIn)]
  public class ExperimentConfiguration : DomainEntity
  {
    [JsonProperty]
    private readonly IList<ISimulationObserver> _statisticalObservers = new List<ISimulationObserver>();

    [JsonProperty]
    public TimeSpan SettlingTime
    {
      get { return Get<TimeSpan>(); }
      set { Set(value); }
    }

    public IEnumerable<ISimulationObserver> StatisticalObservers
    {
      get { return _statisticalObservers; }
    }

    [JsonProperty]
    public DateTime StartTime
    {
      get { return Get<DateTime>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public int NumberOfRuns
    {
      get { return Get<int>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public int Seed
    {
      get { return Get<int>(); }
      set { Set(value); }
    }

    public void AddStatisticalObserver(ISimulationObserver observer)
    {
      _statisticalObservers.Add(observer);
    }

    public void RemoveStatisticalObserver(ISimulationObserver observer)
    {
      _statisticalObservers.Remove(observer);
    }
  }
}