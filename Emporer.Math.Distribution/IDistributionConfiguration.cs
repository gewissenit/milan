#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel;

namespace Emporer.Math.Distribution
{
  public interface IDistributionConfiguration : INotifyPropertyChanged
  {
    string Name { get; }
  }
}