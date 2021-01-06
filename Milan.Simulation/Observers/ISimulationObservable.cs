#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Milan.Simulation.Observers
{
  public interface ISimulationObservable
  {
    [JsonProperty]
    IEnumerable<ISimulationObserver> Observers { get; }

    void Add(ISimulationObserver observer);
    void Remove(ISimulationObserver observer);
  }
}