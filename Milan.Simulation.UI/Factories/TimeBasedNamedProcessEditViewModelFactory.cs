#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel.Composition;
using Emporer.Math.Distribution.UI.Factories;
using Emporer.WPF.ViewModels;
using Milan.Simulation.UI.ViewModels;

namespace Milan.Simulation.UI.Factories
{
  [Export(typeof (INamedProcessViewModelFactory))]
  internal class TimeBasedNamedProcessEditViewModelFactory : INamedProcessViewModelFactory
  {
    private readonly IDistributionConfigurationViewModelFactory _distributionConfigurationViewModelFactory;

    [ImportingConstructor]
    public TimeBasedNamedProcessEditViewModelFactory([Import] IDistributionConfigurationViewModelFactory distributionConfigurationViewModelFactory)
    {
      _distributionConfigurationViewModelFactory = distributionConfigurationViewModelFactory;
    }

    public bool CanHandle(object model)
    {
      return model.GetType() == typeof (TimeBasedNamedProcessConfiguration);
    }

    public IEditViewModel CreateEditViewModel(object model)
    {
      return new TimeBasedNamedProcessEditViewModel(model as TimeBasedNamedProcessConfiguration, _distributionConfigurationViewModelFactory);
    }
  }
}