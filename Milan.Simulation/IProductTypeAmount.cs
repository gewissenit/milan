#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel;

namespace Milan.Simulation
{
  public interface IProductTypeAmount : INotifyPropertyChanged
  {
    IProductType ProductType { get; set; }
    int Amount { get; set; }
  }
}