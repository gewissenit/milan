#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel;
using Caliburn.Micro;
using Emporer.WPF.ViewModels;
using Milan.Simulation;

namespace Milan.UI.ViewModels
{
  public class VisualModelingEntityViewModel : Screen, IViewModel
  {
    private readonly IEntity _entity;

    public VisualModelingEntityViewModel(IEntity entity)
    {
      _entity = entity;
      DisplayName = _entity.Name;
      _entity.PropertyChanged += UpdateName;
    }

    private void UpdateName(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName!="Name")
      {
        return;
      }

      DisplayName = _entity.Name;
    }

    public object Model
    {
      get { return _entity; }
    }
  }
}