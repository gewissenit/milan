#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Caliburn.Micro;
using Milan.Simulation.Factories;

namespace Milan.Simulation.UI.ViewModels
{
  public class StatisticalObserverFactoryViewModel : PropertyChangedBase
  {
    private readonly IStatisticalObserverFactory _factory;
    private bool _isSelected;

    public StatisticalObserverFactoryViewModel(IStatisticalObserverFactory factory, bool isSelected)
    {
      _factory = factory;
      _isSelected = isSelected;
    }

    public IStatisticalObserverFactory Model
    {
      get { return _factory; }
    }

    public string Name { get { return _factory.Name; } }

    public bool IsSelected
    {
      get { return _isSelected; }
      set
      {
        if (_isSelected == value)
        {
          return;
        }
        _isSelected = value;
        NotifyOfPropertyChange(() => IsSelected);
      }
    }
  }
}