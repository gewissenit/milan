#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Emporer.WPF.ViewModels;

namespace Milan.Simulation.UI.ViewModels
{
  public abstract class EntityEditViewModel : EditViewModel
  {
    private readonly IEntity _entity;

    public EntityEditViewModel(IEntity model, string name)
      : base(model, name)
    {
      _entity = model;
    }

    public string Name
    {
      get { return _entity.Name; }
      set
      {
        if (_entity.Name == value)
        {
          return;
        }
        _entity.Name = value;
        NotifyOfPropertyChange(() => Name);
      }
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