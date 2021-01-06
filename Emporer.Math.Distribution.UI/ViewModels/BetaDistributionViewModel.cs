using Emporer.WPF.ViewModels;

namespace Emporer.Math.Distribution.UI.ViewModels
{
  public class BetaDistributionViewModel : DistributionConfigurationViewModel
  {
    private readonly BetaDistributionConfiguration _distributionConfiguration;

    public BetaDistributionViewModel(BetaDistributionConfiguration distributionConfiguration)
      : base(distributionConfiguration)
    {
      _distributionConfiguration = distributionConfiguration;

      Minimum = new DoublePropertyWrapper(() => _distributionConfiguration.Minimum, value => _distributionConfiguration.Minimum = value);
      Maximum = new DoublePropertyWrapper(() => _distributionConfiguration.Maximum, value => _distributionConfiguration.Maximum = value);
      FirstShape = new DoublePropertyWrapper(() => _distributionConfiguration.FirstShape, value => _distributionConfiguration.FirstShape = value);
      SecondShape = new DoublePropertyWrapper(() => _distributionConfiguration.SecondShape, value => _distributionConfiguration.SecondShape = value);
    }

    public DoublePropertyWrapper FirstShape { get; private set; }
    public DoublePropertyWrapper SecondShape { get; private set; }
    public DoublePropertyWrapper Minimum { get; private set; }
    public DoublePropertyWrapper Maximum { get; private set; }
  }
}