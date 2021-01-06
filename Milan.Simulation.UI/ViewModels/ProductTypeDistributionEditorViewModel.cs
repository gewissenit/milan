#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Caliburn.Micro;
using Emporer.Math.Distribution;
using Emporer.Math.Distribution.UI.Factories;
using Emporer.Math.Distribution.UI.ViewModels;
using Emporer.WPF.ViewModels;

namespace Milan.Simulation.UI.ViewModels
{
  public class ProductTypeDistributionEditorViewModel : PropertyChangedBase, IEditViewModel
  {
    public ProductTypeDistributionEditorViewModel(IProductTypeDistribution model,
                                                  IDistributionConfigurationViewModelFactory distributionConfigurationViewModelFactory)
    {
      Model = model;

      Distribution = new DistributionSelectorViewModel(distributionConfigurationViewModelFactory,
                                                       new PropertyWrapper<IDistributionConfiguration>(() => model.DistributionConfiguration,
                                                                                                       v => model.DistributionConfiguration = v));

      ProductType = new ProductTypeViewModel(model.ProductType);
    }

    public object Model { get; set; }
    public ProductTypeViewModel ProductType { get; private set; }
    public DistributionSelectorViewModel Distribution { get; private set; }
    public string DisplayName { get; set; }
  }
}