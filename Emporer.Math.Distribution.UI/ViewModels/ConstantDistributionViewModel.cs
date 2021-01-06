#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Emporer.WPF;
using Emporer.WPF.ViewModels;

namespace Emporer.Math.Distribution.UI.ViewModels
{
  public class ConstantDistributionViewModel : DistributionConfigurationViewModel
  {
    // internal reference to model (the dist. cfg)
    private readonly ConstantDistributionConfiguration _constantDistributionConfiguration;

    // model is injected via ctor
    public ConstantDistributionViewModel(ConstantDistributionConfiguration constantDistributionConfiguration)
      : base(constantDistributionConfiguration)
    {
      _constantDistributionConfiguration = constantDistributionConfiguration;

      // wrapping value property in proxy
      // the arguments are delegates calling the getter and setter of the value property
      ConstantValue = new DoublePropertyWrapper(() => _constantDistributionConfiguration.ConstantValue,
                                                value => _constantDistributionConfiguration.ConstantValue = value);
    }

    // proxy for the constant value 
    // it does not raise property changes itself, it will never be set from the outside
    // a DataTemplate binding to this proxy will operate on the proxies Value property
    public DoublePropertyWrapper ConstantValue { get; private set; }
  }
}