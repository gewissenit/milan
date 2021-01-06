#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Emporer.Math.Distribution.UI.Factories;
using Emporer.WPF.Factories;
using Emporer.WPF.ViewModels;
using Milan.JsonStore;

namespace Milan.Simulation.UI.Factories
{
  public abstract class StationaryElementEditViewModelFactory<T> : IEditViewModelFactory
    where T : class, IDomainEntity
  {
    public StationaryElementEditViewModelFactory(IDistributionConfigurationViewModelFactory distributionViewModelFactory)
    {
      DistributionViewModelFactory = distributionViewModelFactory;
    }

    protected IDistributionConfigurationViewModelFactory DistributionViewModelFactory { get; private set; }


    public bool CanHandle(object model)
    {
      return model.GetType() == typeof (T);
    }

    public IEditViewModel CreateEditViewModel(object model)
    {
      return CreateEditViewModel((T) model);
    }

    protected abstract IEditViewModel CreateEditViewModel(T model);
  }
}