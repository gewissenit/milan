#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.ObjectModel;
using Emporer.WPF.ViewModels;

namespace Milan.Simulation.UI.ViewModels
{
  public class EntityViewModel : ReactiveModelWrapper<IEntity>
  {
    public EntityViewModel(IEntity model)
      : base(model)
    {
      if (model == null)
      {
        throw new ArgumentNullException("model");
      }

      Components = new ObservableCollection<IViewModel>();
    }

    public ObservableCollection<IViewModel> Components { get; private set; }
  }
}