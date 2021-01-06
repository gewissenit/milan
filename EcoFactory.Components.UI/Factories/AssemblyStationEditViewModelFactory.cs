#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel.Composition;
using Caliburn.Micro;
using EcoFactory.Components.Factories;
using EcoFactory.Components.UI.ViewModels;
using Emporer.Math.Distribution.UI.Factories;
using Emporer.WPF.Factories;
using Emporer.WPF.ViewModels;
using Milan.Simulation.Factories;
using Milan.Simulation.Resources.Factories;
using Milan.Simulation.UI.Factories;

namespace EcoFactory.Components.UI.Factories
{
  [Export(typeof(IEditViewModelFactory))]
  internal class AssemblyStationEditViewModelFactory : StationaryElementEditViewModelFactory<AssemblyStation>
  {
    private readonly IChainedParameterCommandFactory _chainedParameterCommandFactory;
    private readonly IConnectionFactory _connectionFactory;
    private readonly IDistributionConfigurationViewModelFactory _distributionConfigurationViewModelFactory;
    private readonly IDistributionSelectionViewModelFactory _distributionSelectionViewModelFactory;
    private readonly IProductTypeAmountFactory _productTypeAmountFactory;
    private readonly IProductTypeDistributionFactory _productTypeDistributionFactory;
    private readonly IResourcePoolResourceTypeAmountFactory _resourceFactory;
    private readonly ITransformationRuleFactory _transformationRuleFactory;
    private readonly ITransformationRuleOutputFactory _transformationRuleOutputFactory;

    [ImportingConstructor]
    public AssemblyStationEditViewModelFactory([Import] ITransformationRuleFactory transformationRuleFactory,
                                               [Import] IProductTypeAmountFactory productTypeAmountFactory,
                                               [Import] IProductTypeDistributionFactory productTypeDistributionFactory,
                                               [Import] ITransformationRuleOutputFactory transformationRuleOutputFactory,
                                               [Import] IDistributionConfigurationViewModelFactory distributionViewModelFactory,
                                               [Import] IResourcePoolResourceTypeAmountFactory resourceFactory,
                                               [Import] IConnectionFactory connectionFactory,
                                               [Import] IChainedParameterCommandFactory chainedParameterCommandFactory,
                                               [Import] IDistributionConfigurationViewModelFactory distributionConfigurationViewModelFactory,
                                               [Import] IDistributionSelectionViewModelFactory distributionSelectionViewModelFactory)
      : base(distributionViewModelFactory)
    {
      _transformationRuleFactory = transformationRuleFactory;
      _productTypeAmountFactory = productTypeAmountFactory;
      _productTypeDistributionFactory = productTypeDistributionFactory;
      _transformationRuleOutputFactory = transformationRuleOutputFactory;
      _resourceFactory = resourceFactory;
      _connectionFactory = connectionFactory;
      _chainedParameterCommandFactory = chainedParameterCommandFactory;
      _distributionConfigurationViewModelFactory = distributionConfigurationViewModelFactory;
      _distributionSelectionViewModelFactory = distributionSelectionViewModelFactory;
    }

    protected override IEditViewModel CreateEditViewModel(AssemblyStation model)
    {
      var sections = new Screen[]
                     {
                       new ConnectionSectionViewModel(model, _connectionFactory),
                       new TransformationsSectionViewModel(model,
                                                           _productTypeAmountFactory,
                                                           _transformationRuleOutputFactory,
                                                           _distributionConfigurationViewModelFactory,
                                                           _resourceFactory,
                                                           _chainedParameterCommandFactory,
                                                           _transformationRuleFactory),
                       new SetupSectionViewModel(model, _resourceFactory, _productTypeDistributionFactory, _distributionConfigurationViewModelFactory),
                       new FailureSectionViewModel(model, _distributionSelectionViewModelFactory), new ShiftsSectionViewModel(model),
                     };

      return new AssemblyStationEditViewModel(model, sections);
    }
  }
}