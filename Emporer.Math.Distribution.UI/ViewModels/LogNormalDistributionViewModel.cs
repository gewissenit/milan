#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Emporer.WPF.ViewModels;

namespace Emporer.Math.Distribution.UI.ViewModels
{
  public class LogNormalDistributionViewModel : DistributionConfigurationViewModel
  {
    private readonly LogNormalDistributionConfiguration _distributionConfiguration;

    public LogNormalDistributionViewModel(LogNormalDistributionConfiguration logNormalDistributionConfiguration)
      : base(logNormalDistributionConfiguration)
    {
      _distributionConfiguration = logNormalDistributionConfiguration;

      Mean = new DoublePropertyWrapper(() => _distributionConfiguration.Mean, value => _distributionConfiguration.Mean = value);

      StandardDeviation = new DoublePropertyWrapper(() => _distributionConfiguration.StandardDeviation,
                                                    value => _distributionConfiguration.StandardDeviation = value);

      Minimum = new DoublePropertyWrapper(() => _distributionConfiguration.Minimum, value => _distributionConfiguration.Minimum = value);
    }

    public bool ParamsNormal
    {
      get { return _distributionConfiguration.ParamsNormal; }
      set
      {
        if (_distributionConfiguration.ParamsNormal == value)
        {
          return;
        }
        _distributionConfiguration.ParamsNormal = value;
        NotifyOfPropertyChange(() => ParamsNormal);
      }
    }

    public DoublePropertyWrapper Mean { get; private set; }
    public DoublePropertyWrapper StandardDeviation { get; private set; }
    public DoublePropertyWrapper Minimum { get; private set; }
  }
}