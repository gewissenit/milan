#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel;

namespace Milan.Simulation.Resources
{
  public interface IResourceTypeAmount : INotifyPropertyChanged
  {
    IResourceType ResourceType { get; set; }
    int Amount { get; set; }
  }
}