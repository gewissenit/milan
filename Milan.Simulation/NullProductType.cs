#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.ComponentModel;

namespace Milan.Simulation
{
  /// <summary>
  ///   NullObject used in some view models to avoid mixed elements in product types list.
  /// </summary>
  public class NullProductType : IProductType
  {
    public NullProductType()
    {
      Name = "All";
    }

    public IExperiment CurrentExperiment { get; set; }

    public string IconId { get; set; }
    public Guid Id { get; set; }

    public bool IsReadonly { get; set; }
    public IModel Model { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public event PropertyChangedEventHandler PropertyChanged
    {
      add { }
      remove { }
    }

    public int CompareTo(object obj)
    {
      throw new NotImplementedException();
    }

    public void Initialize()
    {
      throw new NotImplementedException();
    }

    public void Reset()
    {
      throw new NotImplementedException();
    }

    public override string ToString()
    {
      return "ProductType: All (null value)";
    }
  }
}