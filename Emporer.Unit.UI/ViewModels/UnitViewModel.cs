#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Caliburn.Micro;

namespace Emporer.Unit.UI.ViewModels
{
  public class UnitViewModel : PropertyChangedBase
  {
    private readonly IUnit _unit;

    public UnitViewModel(IUnit unit)
    {
      _unit = unit;
    }

    public IUnit Model
    {
      get { return _unit; }
    }
  }
}