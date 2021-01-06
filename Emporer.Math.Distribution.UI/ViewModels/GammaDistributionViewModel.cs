#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Emporer.WPF.ViewModels;

namespace Emporer.Math.Distribution.UI.ViewModels
{
  public class GammaDistributionViewModel : DistributionConfigurationViewModel
  {
    private readonly GammaDistributionConfiguration _distributionConfiguration;

    public GammaDistributionViewModel(GammaDistributionConfiguration gammaDistributionConfiguration)
      : base(gammaDistributionConfiguration)
    {
      _distributionConfiguration = gammaDistributionConfiguration;

      Scale = new DoublePropertyWrapper(() => _distributionConfiguration.Scale, value => _distributionConfiguration.Scale = value);

      Shape = new DoublePropertyWrapper(() => _distributionConfiguration.Shape, value => _distributionConfiguration.Shape = value);
    }

    public DoublePropertyWrapper Scale { get; private set; }
    public DoublePropertyWrapper Shape { get; private set; }
  }
}