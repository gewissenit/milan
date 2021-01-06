#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

namespace Milan.Simulation.UI.ViewModels
{
  public class ProductTypeEditViewModel : EntityEditViewModel
  {
    public ProductTypeEditViewModel(IProductType productType)
      : base(productType, "Product Type")
    {
    }
  }
}