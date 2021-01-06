#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel.Composition;
using Caliburn.Micro;
using EcoFactory.Components.UI.ViewModels;
using Emporer.WPF.Factories;
using Emporer.WPF.ViewModels;
using Milan.Simulation.UI.Factories;

namespace EcoFactory.Components.UI.Factories
{
  [Export(typeof (IEditViewModelFactory))]
  internal class ExitPointEditViewModelFactory : IEditViewModelFactory
  {
    public bool CanHandle(object model)
    {
      return model.GetType() == typeof (ExitPoint);
    }

    public IEditViewModel CreateEditViewModel(object model)
    {
      return new ExitPointEditViewModel(model as IExitPoint);
    }
  }
}