using Emporer.WPF.ViewModels;

namespace Emporer.Math.Distribution.UI.ViewModels
{
  public class BernoulliDistributionViewModel : DistributionConfigurationViewModel
  {
    private readonly BernoulliDistributionConfiguration _distributionConfiguration;

    public BernoulliDistributionViewModel(BernoulliDistributionConfiguration distributionConfiguration)
      : base(distributionConfiguration)
    {
      _distributionConfiguration = distributionConfiguration;

      Probability = new DoublePropertyWrapper(() => _distributionConfiguration.Probability, value => _distributionConfiguration.Probability = value);
    }

    public DoublePropertyWrapper Probability { get; private set; }
  }
}