#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.ObjectModel;
using System.ComponentModel;
using Caliburn.Micro;

namespace Milan.Simulation.UI.ViewModels
{
  public class EntityNavigationViewModel : Screen, INavigatorNode
  {
    private readonly IEntity _model;
    private bool _isExpanded;
    private bool _isSelected;

    public EntityNavigationViewModel(IEntity entity)
    {
      Items = new ObservableCollection<INavigatorNode>();

      _model = entity;
      DisplayName = entity.Name;

      _model.PropertyChanged += UpdateRelatedLocalProperty;
    }

    public object Model
    {
      get { return _model; }
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

    public bool IsExpanded
    {
      get { return _isExpanded; }
      set
      {
        if (_isExpanded == value)
        {
          return;
        }
        _isExpanded = value;
        NotifyOfPropertyChange(() => IsExpanded);
      }
    }

    public ObservableCollection<INavigatorNode> Items { get; private set; }

    private void UpdateRelatedLocalProperty(object sender, PropertyChangedEventArgs e)
    {
      switch (e.PropertyName)
      {
        case "Name":
          DisplayName = _model.Name;
          break;
      }
    }
  }
}