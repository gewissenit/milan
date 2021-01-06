#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Caliburn.Micro;
using Emporer.Unit;

namespace Milan.Simulation.CostAccounting.UI.ViewModels
{
  public class CurrencyViewModel : PropertyChangedBase
  {
    private readonly IUnit _unit;

    public CurrencyViewModel(IUnit unit)
    {
      _unit = unit;
    }

    public IUnit Model
    {
      get { return _unit; }
    }
  }
}