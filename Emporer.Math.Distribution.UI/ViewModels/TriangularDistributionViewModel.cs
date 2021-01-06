using Emporer.WPF.ViewModels;

namespace Emporer.Math.Distribution.UI.ViewModels
{
  public class TriangularDistributionViewModel : DistributionConfigurationViewModel
  {
    private readonly TriangularDistributionConfiguration _distributionConfiguration;

    public TriangularDistributionViewModel(TriangularDistributionConfiguration distributionConfiguration)
      : base(distributionConfiguration)
    {
      _distributionConfiguration = distributionConfiguration;

      LowerBorder = new DoublePropertyWrapper(() => _distributionConfiguration.LowerBorder, value => _distributionConfiguration.LowerBorder = value);
      Mean = new DoublePropertyWrapper(() => _distributionConfiguration.Mean, value => _distributionConfiguration.Mean = value);
      UpperBorder = new DoublePropertyWrapper(() => _distributionConfiguration.UpperBorder, value => _distributionConfiguration.UpperBorder = value);
    }

    public DoublePropertyWrapper Mean { get; private set; }

    public DoublePropertyWrapper LowerBorder { get; private set; }

    public DoublePropertyWrapper UpperBorder { get; private set; }
  }
}