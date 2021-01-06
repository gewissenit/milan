#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Caliburn.Micro;
using Milan.Simulation.Observers;

namespace Milan.Simulation.UI.ViewModels
{
  public class TerminationCriterionViewModel : PropertyChangedBase
  {
    private readonly ITerminationCriteria _criterion;
    private bool _isSelected;

    public TerminationCriterionViewModel(ITerminationCriteria criterion, bool selected)
    {
      _criterion = criterion;
      IsSelected = selected;
    }

    public ITerminationCriteria Model
    {
      get { return _criterion; }
    }

    public string Name
    {
      get { return _criterion.Name; }
    }

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