using Emporer.Math.Distribution.UI.ViewModels;
using Emporer.WPF.ViewModels;

namespace Emporer.Math.Distribution.UI.Factories
{
  public interface IDistributionSelectionViewModelFactory
  {
    DistributionSelectorViewModel Create(PropertyWrapper<IDistributionConfiguration> distributionProperty);
  }
}
