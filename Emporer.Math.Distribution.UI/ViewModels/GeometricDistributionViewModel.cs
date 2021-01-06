using Emporer.WPF.ViewModels;

namespace Emporer.Math.Distribution.UI.ViewModels
{
  public class GeometricDistributionViewModel : DistributionConfigurationViewModel
  {
    private readonly GeometricDistributionConfiguration _distributionConfiguration;

    public GeometricDistributionViewModel(GeometricDistributionConfiguration distributionConfiguration)
      : base(distributionConfiguration)
    {
      _distributionConfiguration = distributionConfiguration;

      Probability = new DoublePropertyWrapper(() => _distributionConfiguration.Probability, value => _distributionConfiguration.Probability = value);
    }

    public DoublePropertyWrapper Probability { get; private set; }
  }
}