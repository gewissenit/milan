#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel;
using Caliburn.Micro;

namespace Milan.Simulation.UI.ViewModels
{
  //todo: implement IEditViewModel
  public class ProductTypeAmountEditorViewModel : PropertyChangedBase
  {
    private readonly IProductTypeAmount _model;

    public ProductTypeAmountEditorViewModel(IProductTypeAmount productTypeAmount)
    {
      _model = productTypeAmount;
      _model.PropertyChanged += ReactToModelChange;
      ProductType = new ProductTypeViewModel(_model.ProductType);
    }

    public IProductTypeAmount Model
    {
      get { return _model; }
    }

    public ProductTypeViewModel ProductType { get; private set; }

    public int Amount
    {
      get { return _model.Amount; }
      set { _model.Amount = value; }
    }

    private void ReactToModelChange(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "Amount")
      {
        NotifyOfPropertyChange(() => Amount);
      }
    }
  }
}