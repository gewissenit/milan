#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel;

namespace Milan.Simulation.Resources
{
  public interface IResourceTypeInfluence : INotifyPropertyChanged
  {
    IInfluence Influence { get; set; }
    double IncreaseFactor { get; set; }
    double RecoveryRate { get; set; }
    double InitialValue { get; set; }
    double LowerLimit { get; set; }
    double UpperLimit { get; set; }
  }
}