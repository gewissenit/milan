#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel.Composition;
using Caliburn.Micro;
using Emporer.WPF.Factories;
using Emporer.WPF.ViewModels;
using Milan.Simulation.UI.ViewModels;

namespace Milan.Simulation.UI.Factories
{
  [Export(typeof(IEditViewModelFactory))]
  internal class ModelEditViewModelFactory : IEditViewModelFactory
  {
    public bool CanHandle(object model)
    {
      return model.GetType() == typeof(Model);
    }

    public IEditViewModel CreateEditViewModel(object model)
    {
      return new ModelEditViewModel(model as IModel,
                                    new Screen[]
                                    {
                                      new StartDateSectionViewModel(model as IModel)
                                    });
    }
  }
}