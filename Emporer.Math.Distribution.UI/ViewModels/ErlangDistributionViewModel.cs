using Emporer.WPF.ViewModels;

namespace Emporer.Math.Distribution.UI.ViewModels
{
  public class ErlangDistributionViewModel : DistributionConfigurationViewModel
  {
    private readonly ErlangDistributionConfiguration _distributionConfiguration;

    public ErlangDistributionViewModel(ErlangDistributionConfiguration distributionConfiguration)
      : base(distributionConfiguration)
    {
      _distributionConfiguration = distributionConfiguration;

      Mean = new DoublePropertyWrapper(() => _distributionConfiguration.Mean, value => _distributionConfiguration.Mean = value);
      Minimum = new DoublePropertyWrapper(() => _distributionConfiguration.Minimum, value => _distributionConfiguration.Minimum = value);
    }

    public DoublePropertyWrapper Mean { get; private set; }

    public int Order
    {
      get { return _distributionConfiguration.Order; }
      set
      {
        if (_distributionConfiguration.Order == value)
        {
          return;
        }
        _distributionConfiguration.Order = value;
        NotifyOfPropertyChange(() => Order);
      }
    }

    public DoublePropertyWrapper Minimum { get; private set; }
  }
}