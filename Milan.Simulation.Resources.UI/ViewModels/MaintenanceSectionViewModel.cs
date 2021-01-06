#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using Caliburn.Micro;
using Emporer.Math.Distribution;
using Emporer.Math.Distribution.UI.Factories;
using Emporer.Math.Distribution.UI.ViewModels;
using Emporer.WPF.ViewModels;

namespace Milan.Simulation.Resources.UI.ViewModels
{
  public class MaintenanceSectionViewModel : Screen
  {
    private readonly IDistributionConfigurationViewModelFactory _distributionConfigurationViewModelFactory;

    public MaintenanceSectionViewModel(IResourceType resourceType,
                                       IDistributionConfigurationViewModelFactory distributionConfigurationViewModelFactory)
    {
      DisplayName = "maintenance";
      _distributionConfigurationViewModelFactory = distributionConfigurationViewModelFactory;

      MaintenanceDuration = WrapDistribution(() => resourceType.MaintenanceDuration,
                                             v => resourceType.MaintenanceDuration = v);
      UsageAmount = WrapDistribution(() => resourceType.UsageAmount,
                                     v => resourceType.UsageAmount = v);
    }

    public DistributionSelectorViewModel MaintenanceDuration { get; set; }
    public DistributionSelectorViewModel UsageAmount { get; set; }


    protected DistributionSelectorViewModel WrapDistribution(Func<IDistributionConfiguration> getValue,
                                                             Action<IDistributionConfiguration> setValue)
    {
      return new DistributionSelectorViewModel(_distributionConfigurationViewModelFactory,
                                               new PropertyWrapper<IDistributionConfiguration>(getValue,
                                                                                               setValue));
    }
  }
}