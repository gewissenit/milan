using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Emporer.Math.Distribution.UI.Factories;
using Milan.Simulation.UI.ViewModels;

namespace Milan.Simulation.UI.Factories
{
  internal class ProductTypeDistributionEditorViewModelFactory
  {
    private readonly IDistributionConfigurationViewModelFactory _distributionConfigurationViewModelFactory;

    public ProductTypeDistributionEditorViewModelFactory(IDistributionConfigurationViewModelFactory distributionConfigurationViewModelFactory)
    {
      _distributionConfigurationViewModelFactory = distributionConfigurationViewModelFactory;
    }

    internal ObservableCollection<ProductTypeDistributionEditorViewModel> WrapProductTypeDistributions(
      IEnumerable<IProductTypeDistribution> productTypeDistributions)
    {
      // create a vm for each PT related distribution
      var vms =
        productTypeDistributions.Select(ptDist => new ProductTypeDistributionEditorViewModel(ptDist, _distributionConfigurationViewModelFactory));
      return new ObservableCollection<ProductTypeDistributionEditorViewModel>(vms);
    }
  }
}