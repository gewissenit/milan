#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Caliburn.Micro;
using Emporer.WPF.ViewModels;
using Milan.Simulation.UI.ViewModels;

namespace EcoFactory.Components.UI.ViewModels
{
  public class ExitPointEditViewModel : Screen, IEditViewModel
  {
    private IExitPoint _model;

    public ExitPointEditViewModel(IExitPoint model)
    {
      _model = model;
      DisplayName = _model.Name;
    }

    public string Name
    {
      get { return _model.Name; }
      set
      {
        if (_model.Name == value)
        {
          return;
        }
        _model.Name = value;
        NotifyOfPropertyChange(() => Name);
      }
    }

    public string Description
    {
      get { return _model.Description; }
      set
      {
        if (_model.Description == value)
        {
          return;
        }
        _model.Description = value;
        NotifyOfPropertyChange(() => Description);
      }
    }

    public object Model => _model;
  }
}