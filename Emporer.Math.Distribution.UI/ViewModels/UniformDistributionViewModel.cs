#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Emporer.WPF.ViewModels;

namespace Emporer.Math.Distribution.UI.ViewModels
{
  public class UniformDistributionViewModel : DistributionConfigurationViewModel
  {
    private readonly UniformDistributionConfiguration _distributionConfiguration;

    public UniformDistributionViewModel(UniformDistributionConfiguration uniformDistributionConfiguration)
      : base(uniformDistributionConfiguration)
    {
      _distributionConfiguration = uniformDistributionConfiguration;

      LowerBorder = new DoublePropertyWrapper(() => _distributionConfiguration.LowerBorder,
                                              value => _distributionConfiguration.LowerBorder = value);

      UpperBorder = new DoublePropertyWrapper(() => _distributionConfiguration.UpperBorder,
                                              value => _distributionConfiguration.UpperBorder = value);
    }

    public DoublePropertyWrapper LowerBorder { get; private set; }
    public DoublePropertyWrapper UpperBorder { get; private set; }
  }
}