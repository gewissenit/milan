#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Emporer.WPF.ViewModels;

namespace Emporer.Math.Distribution.UI.ViewModels
{
  public class NormalDistributionViewModel : DistributionConfigurationViewModel
  {
    private readonly NormalDistributionConfiguration _distributionConfiguration;

    public NormalDistributionViewModel(NormalDistributionConfiguration normalDistributionConfiguration)
      : base(normalDistributionConfiguration)
    {
      _distributionConfiguration = normalDistributionConfiguration;

      Mean = new DoublePropertyWrapper(() => _distributionConfiguration.Mean, value => _distributionConfiguration.Mean = value);

      StandardDeviation = new DoublePropertyWrapper(() => _distributionConfiguration.StandardDeviation,
                                                    value => _distributionConfiguration.StandardDeviation = value);
    }

    public DoublePropertyWrapper Mean { get; private set; }
    public DoublePropertyWrapper StandardDeviation { get; private set; }
  }
}