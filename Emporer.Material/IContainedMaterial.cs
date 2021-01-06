#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel;

namespace Emporer.Material
{
  public interface IContainedMaterial : INotifyPropertyChanged
  {
    double Amount { get; set; }
    IMaterial Material { get; set; }
  }
}