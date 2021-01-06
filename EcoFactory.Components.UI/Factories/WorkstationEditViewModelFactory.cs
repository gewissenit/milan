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
using Milan.Simulation.Resources.Factories;
using Milan.Simulation.UI.Factories;

namespace EcoFactory.Components.UI.Factories
{
  [Export(typeof(IEditViewModelFactory))]
  internal class WorkstationEditViewModelFactory : StationaryElementEditViewModelFactory<Workstation>
  {
    private readonly IChainedParameterCommandFactory _chainedParameterCommandFactory;
    private readonly IConnectionFactory _connectionFactory;
    private readonly IDistributionSelectionViewModelFactory _distributionSelectionViewModelFactory;
    private readonly IDistributionConfigurationViewModelFactory _distributionViewModelFactory;
    private readonly IProductTypeDistributionFactory _productTypeDistributionFactory;
    private readonly IProductTypeSpecificResourceFactory _productTypeSpecificResourcesFactory;
    private readonly IResourcePoolResourceTypeAmountFactory _resourceFactory;

    [ImportingConstructor]
    public WorkstationEditViewModelFactory([Import] IProductTypeDistributionFactory productTypeDistributionFactory,
                                           [Import] IDistributionConfigurationViewModelFactory distributionViewModelFactory,
                                           [Import] IDistributionSelectionViewModelFactory distributionSelectionViewModelFactory,
                                           [Import] IProductTypeSpecificResourceFactory productTypeSpecificResourcesFactory,
                                           [Import] IResourcePoolResourceTypeAmountFactory resourceFactory,
                                           [Import] IConnectionFactory connectionFactory,
                                           [Import] IChainedParameterCommandFactory chainedParameterCommandFactory)
      : base(distributionViewModelFactory)

    {
      _productTypeDistributionFactory = productTypeDistributionFactory;
      _distributionViewModelFactory = distributionViewModelFactory;
      _distributionSelectionViewModelFactory = distributionSelectionViewModelFactory;
      _productTypeSpecificResourcesFactory = productTypeSpecificResourcesFactory;
      _resourceFactory = resourceFactory;
      _connectionFactory = connectionFactory;
      _chainedParameterCommandFactory = chainedParameterCommandFactory;
    }

    protected override IEditViewModel CreateEditViewModel(Workstation model)
    {
      var sections = new Screen[]
                     {
                       new ConnectionSectionViewModel(model, _connectionFactory),
                       new ProcessingSectionViewModel(model,
                                                      _resourceFactory,
                                                      _productTypeSpecificResourcesFactory,
                                                      _productTypeDistributionFactory,
                                                      _distributionViewModelFactory,
                                                      _distributionSelectionViewModelFactory,
                                                      _chainedParameterCommandFactory),
                       new SetupSectionViewModel(model,
                                                 _resourceFactory,
                                                 _productTypeDistributionFactory,
                                                 _distributionViewModelFactory),
                       new FailureSectionViewModel(model, _distributionSelectionViewModelFactory),
                       new InfluencesSectionViewModel(model),
                       new ShiftsSectionViewModel(model)
                     };


      return new WorkstationEditViewModel(model, sections);
    }
  }
}