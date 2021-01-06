#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Emporer.WPF.ViewModels;

namespace Emporer.Math.Distribution.UI.ViewModels
{
  public class ExponentialDistributionViewModel : DistributionConfigurationViewModel
  {
    private readonly ExponentialDistributionConfiguration _distributionConfiguration;

    public ExponentialDistributionViewModel(ExponentialDistributionConfiguration exponentialDistributionConfiguration)
      : base(exponentialDistributionConfiguration)
    {
      _distributionConfiguration = exponentialDistributionConfiguration;

      Mean = new DoublePropertyWrapper(() => _distributionConfiguration.Mean, value => _distributionConfiguration.Mean = value);

      Minimum = new DoublePropertyWrapper(() => _distributionConfiguration.Minimum, value => _distributionConfiguration.Minimum = value);
    }

    public DoublePropertyWrapper Mean { get; private set; }
    public DoublePropertyWrapper Minimum { get; private set; }
  }
}