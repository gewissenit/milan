#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel.Composition;
using Caliburn.Micro;
using EcoFactory.Components.UI.ViewModels;
using Emporer.Math.Distribution.UI.Factories;
using Emporer.WPF.Factories;
using Emporer.WPF.ViewModels;
using Milan.Simulation.Factories;
using Milan.Simulation.UI.Factories;

namespace EcoFactory.Components.UI.Factories
{
  [Export(typeof(IEditViewModelFactory))]
  internal class StorageEditViewModelFactory : StationaryElementEditViewModelFactory<Storage>
  {
    private readonly IChainedParameterCommandFactory _chainedParameterCommandFactory;
    private readonly IConnectionFactory _connectionFactory;
    private readonly IProductTypeAmountFactory _productTypeAmmountFactory;

    [ImportingConstructor]
    public StorageEditViewModelFactory([Import] IProductTypeAmountFactory productTypeAmmountFactory,
                                       [Import] IConnectionFactory connectionFactory,
                                       [Import] IChainedParameterCommandFactory chainedParameterCommandFactory,
                                       [Import] IDistributionConfigurationViewModelFactory distributionViewModelFactory)
      : base(distributionViewModelFactory)
    {
      _productTypeAmmountFactory = productTypeAmmountFactory;
      _connectionFactory = connectionFactory;
      _chainedParameterCommandFactory = chainedParameterCommandFactory;
    }

    protected override IEditViewModel CreateEditViewModel(Storage model)
    {
      return new StorageEditViewModel(model,
                                      new Screen[]
                                      {
                                        new ConnectionSectionViewModel(model, _connectionFactory),
                                        new CapacitySectionViewModel(model, _productTypeAmmountFactory, _chainedParameterCommandFactory)
                                      });
    }
  }
}