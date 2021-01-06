#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel.Composition;
using Caliburn.Micro;
using Emporer.WPF.Factories;
using Emporer.WPF.ViewModels;
using Milan.Simulation.Observers;
using Milan.Simulation.UI.ViewModels;

namespace Milan.Simulation.UI.Factories
{
  [Export(typeof (IEditViewModelFactory))]
  internal class TimeTerminationCriteriaEditViewModelFactory : IEditViewModelFactory
  {
    public bool CanHandle(object model)
    {
      return model.GetType() == typeof (TimeTerminationCriteria);
    }

    public IEditViewModel CreateEditViewModel(object model)
    {
      return new TimeTerminationCriteriaEditViewModel(model as ITimeTerminationCriteria);
    }
  }
}