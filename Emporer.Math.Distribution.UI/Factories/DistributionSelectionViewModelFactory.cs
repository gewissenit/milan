using System.ComponentModel.Composition;
using Emporer.Math.Distribution.UI.ViewModels;
using Emporer.WPF.ViewModels;

namespace Emporer.Math.Distribution.UI.Factories
{
  [Export(typeof(IDistributionSelectionViewModelFactory))]
  internal class DistributionSelectionViewModelFactory : IDistributionSelectionViewModelFactory
  {
    private readonly IDistributionConfigurationViewModelFactory _distributionConfigurationViewModelFactory;

    [ImportingConstructor]
    public DistributionSelectionViewModelFactory([Import] IDistributionConfigurationViewModelFactory distributionConfigurationViewModelFactory)
    {
      _distributionConfigurationViewModelFactory = distributionConfigurationViewModelFactory;
    }

    public DistributionSelectorViewModel Create(PropertyWrapper<IDistributionConfiguration> distributionProperty)
    {
      return new DistributionSelectorViewModel(_distributionConfigurationViewModelFactory, distributionProperty);
    }
  }
}