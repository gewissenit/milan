using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using Emporer.Math.Distribution.UI.Factories;
using Emporer.WPF.ViewModels;

namespace Milan.Simulation.UI.Factories
{
  [Export(typeof(IChainedParameterCommandFactory))]
  internal class ChainedParameterCommandFactory : IChainedParameterCommandFactory
  {
    //TODO(PJ): this is still far too complicated. The parameter names are duplicated at least in ArrivalsSectionViewModel.
    private const string ProductTypeParam = "productType";
    private const string DistributionParam = "distribution";
    private readonly IDistributionConfigurationViewModelFactory _distributionConfigurationViewModelFactory;

    [ImportingConstructor]
    public ChainedParameterCommandFactory([Import] IDistributionConfigurationViewModelFactory distributionConfigurationViewModelFactory)
    {
      _distributionConfigurationViewModelFactory = distributionConfigurationViewModelFactory;
    }

    public ChainedParameterCommandViewModel CreateAddProductTypeSpecificAmountCommand(string displayText,
                                                                                      ObservableCollection<object> availableProductTypes,
                                                                                      Action<IDictionary<string, object>> addAction)
    {
      var productTypeParameterSet = new ParameterSet("productType", "Select a product type", availableProductTypes);

      var chainedParameterCommand = new ChainedParameterCommandViewModel(new[]
                                                                         {
                                                                           productTypeParameterSet
                                                                         },
                                                                         addAction)
                                    {
                                      DisplayText = displayText
                                    };

      return chainedParameterCommand;
    }

    public ChainedParameterCommandViewModel CreateAddProductTypeSpecificDistributionCommand(string displayText,
                                                                                            ObservableCollection<object> availableProductTypes,
                                                                                            Action<IDictionary<string, object>> addAction)
    {
      var productTypeParameterSet = new ParameterSet(ProductTypeParam, "Select a product type", availableProductTypes);

      var distributionsParameterSet = new ParameterSet(DistributionParam,
                                                       "Select a distribution type",
                                                       new ObservableCollection<object>(
                                                                                        _distributionConfigurationViewModelFactory
                                                                                          .GetDescriptorsOfAllAvailableDistributions()
                                                                                          .ToArray()));

      var chainedParameterCommand = new ChainedParameterCommandViewModel(new[]
                                                                         {
                                                                           productTypeParameterSet, distributionsParameterSet
                                                                         },
                                                                         addAction)
                                    {
                                      DisplayText = displayText
                                    };

      return chainedParameterCommand;
    }
  }
}