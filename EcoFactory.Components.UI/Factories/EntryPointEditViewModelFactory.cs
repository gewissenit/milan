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
  internal class EntryPointEditViewModelFactory : StationaryElementEditViewModelFactory<EntryPoint>
  {
    private readonly IChainedParameterCommandFactory _chainedParameterCommandFactory;
    private readonly IConnectionFactory _connectionFactory;
    private readonly IDistributionConfigurationViewModelFactory _distributionConfigurationViewModelFactory;
    private readonly IDistributionSelectionViewModelFactory _distributionSelectionViewModelFactory;
    private readonly IProductTypeDistributionFactory _productTypeDistributionFactory;

    [ImportingConstructor]
    public EntryPointEditViewModelFactory([Import] IProductTypeDistributionFactory productTypeDistributionFactory,
                                          [Import] IDistributionConfigurationViewModelFactory distributionViewModelFactory,
                                          [Import] IConnectionFactory connectionFactory,
                                          [Import] IDistributionConfigurationViewModelFactory distributionConfigurationViewModelFactory,
                                          [Import] IDistributionSelectionViewModelFactory distributionSelectionViewModelFactory,
                                          [Import] IChainedParameterCommandFactory chainedParameterCommandFactory)
      : base(distributionViewModelFactory)
    {
      _productTypeDistributionFactory = productTypeDistributionFactory;
      _connectionFactory = connectionFactory;
      _distributionConfigurationViewModelFactory = distributionConfigurationViewModelFactory;
      _distributionSelectionViewModelFactory = distributionSelectionViewModelFactory;
      _chainedParameterCommandFactory = chainedParameterCommandFactory;
    }

    protected override IEditViewModel CreateEditViewModel(EntryPoint model)
    {
      var sections = new Screen[]
                     {
                       new ConnectionSectionViewModel(model, _connectionFactory),
                       new ArrivalsSectionViewModel(model,
                                                    _distributionConfigurationViewModelFactory,
                                                    _productTypeDistributionFactory,
                                                    _distributionSelectionViewModelFactory,
                                                    _chainedParameterCommandFactory),
                       new ShiftsSectionViewModel(model),
                     };


      return new EntryPointEditViewModel(model, sections);
    }
  }
}