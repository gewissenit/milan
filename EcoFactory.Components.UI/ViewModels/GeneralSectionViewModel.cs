#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Caliburn.Micro;
using Milan.Simulation;

namespace EcoFactory.Components.UI.ViewModels
{
  public class GeneralSectionViewModel : Screen
  {
    private readonly IEntity _entity;

    public GeneralSectionViewModel(IEntity model)
    {
      DisplayName = "general";
      _entity = model;
    }

    public string Description
    {
      get { return _entity.Description; }
      set
      {
        if (_entity.Description == value)
        {
          return;
        }
        _entity.Description = value;
        NotifyOfPropertyChange(() => Description);
      }
    }
  }
}