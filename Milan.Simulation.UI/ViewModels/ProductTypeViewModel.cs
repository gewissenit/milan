#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel;
using Caliburn.Micro;

namespace Milan.Simulation.UI.ViewModels
{
  public class ProductTypeViewModel : PropertyChangedBase
  {
    private readonly IProductType _productType;

    public ProductTypeViewModel(IProductType productType)
    {
      _productType = productType;
      _productType.PropertyChanged += ReactToModelChange;
    }


    public IProductType Model
    {
      get { return _productType; }
    }

    public string Name
    {
      get { return _productType.Name; }
      set { _productType.Name = value; }
    }

    public string Description
    {
      get { return _productType.Description; }
      set { _productType.Description = value; }
    }

    private void ReactToModelChange(object sender, PropertyChangedEventArgs e)
    {
      switch (e.PropertyName)
      {
        case "Name":
          NotifyOfPropertyChange(() => Name);
          return;
        case "Description":
          NotifyOfPropertyChange(() => Description);
          return;
      }
    }
  }
}