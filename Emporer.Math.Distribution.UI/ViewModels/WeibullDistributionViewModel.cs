#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Emporer.WPF;
using Emporer.WPF.ViewModels;

namespace Emporer.Math.Distribution.UI.ViewModels
{
  public class WeibullDistributionViewModel : DistributionConfigurationViewModel
  {
    private readonly WeibullDistributionConfiguration _weibullDistributionConfiguration;

    public WeibullDistributionViewModel(WeibullDistributionConfiguration weibullDistributionConfiguration)
      : base(weibullDistributionConfiguration)
    {
      _weibullDistributionConfiguration = weibullDistributionConfiguration;

      Scale = new DoublePropertyWrapper(() => _weibullDistributionConfiguration.Scale, value => _weibullDistributionConfiguration.Scale = value);

      Shape = new DoublePropertyWrapper(() => _weibullDistributionConfiguration.Shape, value => _weibullDistributionConfiguration.Shape = value);
    }

    public DoublePropertyWrapper Scale { get; private set; }
    public DoublePropertyWrapper Shape { get; private set; }
  }
}