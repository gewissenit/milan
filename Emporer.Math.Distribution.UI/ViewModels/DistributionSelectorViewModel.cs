#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using Emporer.Math.Distribution.UI.Factories;
using Emporer.WPF.ViewModels;

namespace Emporer.Math.Distribution.UI.ViewModels
{
  public class DistributionSelectorViewModel : PropertyChangedBase
  {
    private readonly IDistributionConfigurationViewModelFactory _distributionConfigurationViewModelFactory;
    private readonly PropertyWrapper<IDistributionConfiguration> _targetWrapper;
    private IEnumerable<DistributionConfigurationViewModel> _distributions;
    private DistributionConfigurationViewModel _selectedDistribution;

    public DistributionSelectorViewModel(IDistributionConfigurationViewModelFactory distributionConfigurationViewModelFactory,
                                         PropertyWrapper<IDistributionConfiguration> targetWrapper)
    {
      _distributionConfigurationViewModelFactory = distributionConfigurationViewModelFactory;
      _targetWrapper = targetWrapper;
      if (targetWrapper == null)
      {
        throw new ArgumentNullException(nameof(targetWrapper));
      }

      _selectedDistribution = _distributionConfigurationViewModelFactory.CreateMatchingViewModel(targetWrapper.Value);
      Distributions = GetInitialDistributions();
    }

    public IEnumerable<DistributionConfigurationViewModel> Distributions
    {
      get { return _distributions; }
      set
      {
        if (_distributions == value)
        {
          return;
        }
        _distributions = value;
        NotifyOfPropertyChange(() => Distributions);
      }
    }

    public DistributionConfigurationViewModel SelectedDistribution
    {
      get { return _selectedDistribution; }
      set
      {
        if (_selectedDistribution == value)
        {
          return;
        }
        _selectedDistribution = value;
        _targetWrapper.Value = value.Entity;
        NotifyOfPropertyChange(() => SelectedDistribution);
      }
    }

    private IEnumerable<DistributionConfigurationViewModel> GetInitialDistributions()
    {
      // get an instance for every available type of distribution
      var viewModelsForAllDistributions = _distributionConfigurationViewModelFactory.CreateViewModelForEachDistributionType();

      if (_selectedDistribution == null)
      {
        return viewModelsForAllDistributions;
      }

      // remove the newly created instance that matches the selected dist and use the selected one instead
      return viewModelsForAllDistributions.Where(vm => vm.Entity.GetType() != _selectedDistribution.Entity.GetType())
                                          .Concat(new[]
                                                  {
                                                    _selectedDistribution
                                                  });
    }
  }
}