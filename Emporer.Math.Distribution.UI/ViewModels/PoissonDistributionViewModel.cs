#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Emporer.WPF;
using Emporer.WPF.ViewModels;

namespace Emporer.Math.Distribution.UI.ViewModels
{
  public class PoissonDistributionViewModel : DistributionConfigurationViewModel
  {
    private readonly PoissonDistributionConfiguration _distributionConfiguration;

    public PoissonDistributionViewModel(PoissonDistributionConfiguration poissonDistributionConfiguration)
      : base(poissonDistributionConfiguration)
    {
      _distributionConfiguration = poissonDistributionConfiguration;

      Mean = new DoublePropertyWrapper(() => _distributionConfiguration.Mean, value => _distributionConfiguration.Mean = value);

      ExpMean = new DoublePropertyWrapper(() => _distributionConfiguration.ExpMean,
                                          value => _distributionConfiguration.ExpMean = value);
    }

    public DoublePropertyWrapper Mean { get; private set; }
    public DoublePropertyWrapper ExpMean { get; private set; }
  }
}