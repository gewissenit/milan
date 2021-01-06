#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel.Composition;
using Caliburn.Micro;
using Emporer.WPF.Factories;
using Emporer.WPF.ViewModels;
using Milan.Simulation.Factories;
using Milan.Simulation.Observers;
using Milan.Simulation.UI.ViewModels;

namespace Milan.Simulation.UI.Factories
{
  [Export(typeof (IEditViewModelFactory))]
  internal class ProductTerminationCriteriaEditViewModelFactory : IEditViewModelFactory
  {
    private readonly IProductTypeAmountFactory _productTypeAmountFactory;

    [ImportingConstructor]
    public ProductTerminationCriteriaEditViewModelFactory([Import] IProductTypeAmountFactory productTypeAmountFactory)
    {
      _productTypeAmountFactory = productTypeAmountFactory;
    }

    public bool CanHandle(object model)
    {
      return model.GetType() == typeof (ProductTerminationCriteria);
    }

    public IEditViewModel CreateEditViewModel(object model)
    {
      return new ProductTerminationCriteriaEditViewModel(model as IProductTerminationCriteria, _productTypeAmountFactory);
    }
  }
}