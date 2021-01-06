#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Emporer.WPF.Commands;
using Emporer.WPF.ViewModels;
using Milan.Simulation.Factories;
using Milan.Simulation.Observers;

namespace Milan.Simulation.UI.ViewModels
{
  public class ProductTerminationCriteriaEditViewModel : EditViewModel
  {
    private readonly ObservableCollection<object> _availableProductTypesForSpecificCapacities;
    private readonly IProductTerminationCriteria _model;
    private readonly IProductTypeAmountFactory _productTypeAmountFactory;

    public ProductTerminationCriteriaEditViewModel(IProductTerminationCriteria model, IProductTypeAmountFactory productTypeAmountFactory)
      : base(model, "Product Termination Criteria")
    {
      _model = model;
      _productTypeAmountFactory = productTypeAmountFactory;

      ProductTypeSpecificAmounts = InitializeProductTypeRelatedCapacities(model.ProductAmounts);
      _availableProductTypesForSpecificCapacities =
        new ObservableCollection<object>(GetAvailableProductTypes(ProductTypeSpecificAmounts.Select(ptsd => ptsd.ProductType.Model)));
      AddProductTypeSpecificAmountCommand = CreateAddProductTypeSpecificAmountCommandItem("Add",
                                                                                          _availableProductTypesForSpecificCapacities,
                                                                                          AddAmount);
      RemoveCommand = new RelayCommand(Remove);
    }

    public ICommand RemoveCommand { get; private set; }

    public ObservableCollection<ProductTypeAmountEditorViewModel> ProductTypeSpecificAmounts { get; set; }

    public ChainedParameterCommandViewModel AddProductTypeSpecificAmountCommand { get; set; }

    public bool HasAndOperator
    {
      get { return _model.HasAndOperator; }
      set
      {
        if (_model.HasAndOperator == value)
        {
          return;
        }
        _model.HasAndOperator = value;
        NotifyOfPropertyChange(() => HasAndOperator);
      }
    }

    public void Remove(object item)
    {
      var pt = item as ProductTypeAmountEditorViewModel;

      if (pt == null)
      {
        return;
      }

      if (TryRemoveItem(pt, ProductTypeSpecificAmounts, RemoveAmount))
      {
        return;
      }
    }

    private IEnumerable<IProductType> GetAvailableProductTypes(IEnumerable<IProductType> productTypesAlreadyInUse)
    {
      return _model.Model.Entities.OfType<IProductType>()
                    .Except(productTypesAlreadyInUse);
    }

    private ChainedParameterCommandViewModel CreateAddProductTypeSpecificAmountCommandItem(string displayText,
                                                                                              ObservableCollection<object> availableProductTypes,
                                                                                              Action<IDictionary<string, object>> addAction)
    {
      var productTypeParameterSet = new ParameterSet("productType", "Select a product type", availableProductTypes);

      var chainedParameterCommand = new ChainedParameterCommandViewModel(new[]
                                                                            {
                                                                              productTypeParameterSet
                                                                            },
                                                                            addAction)
                                    {
                                      DisplayText = displayText
                                    };

      return chainedParameterCommand;
    }

    private ObservableCollection<ProductTypeAmountEditorViewModel> InitializeProductTypeRelatedCapacities(
      IEnumerable<IProductTypeAmount> productTypeAmounts)
    {
      // create a vm for each PT related distribution
      var vms = productTypeAmounts.Select(ptDist => new ProductTypeAmountEditorViewModel(ptDist));
      return new ObservableCollection<ProductTypeAmountEditorViewModel>(vms);
    }

    private void AddAmount(IDictionary<string, object> parameters)
    {
      AddProductTypeSpecificCapacity(parameters,
                                     vm =>
                                     {
                                       ProductTypeSpecificAmounts.Add(vm);
                                       // remove product type from selectable parameter values (so it can't be selected twice)
                                       _availableProductTypesForSpecificCapacities.Remove(vm.ProductType.Model);
                                     },
                                     _model.Add);
    }

    private void AddProductTypeSpecificCapacity(IDictionary<string, object> parameters,
                                                Action<ProductTypeAmountEditorViewModel> updateViewModel,
                                                Action<IProductTypeAmount> addToModel)
    {
      // extract product type from parameter set
      var productType = (IProductType) parameters["productType"];

      // use values to create new entry, add to model (workstation)
      var ptSpecific = _productTypeAmountFactory.Create();
      ptSpecific.ProductType = productType;
      addToModel(ptSpecific);

      var vm = new ProductTypeAmountEditorViewModel(ptSpecific);

      updateViewModel(vm);
    }

    private bool TryRemoveItem(ProductTypeAmountEditorViewModel item,
                               ObservableCollection<ProductTypeAmountEditorViewModel> listOfViewModels,
                               Action<IProductTypeAmount> removeFromModel)
    {
      if (listOfViewModels.Contains(item))
      {
        listOfViewModels.Remove(item);
        removeFromModel(item.Model);
        return true;
      }
      return false;
    }

    private void RemoveAmount(IProductTypeAmount productTypeAmount)
    {
      _model.Remove(productTypeAmount);
      _availableProductTypesForSpecificCapacities.Add(productTypeAmount.ProductType);
      NotifyOfPropertyChange(() => AddProductTypeSpecificAmountCommand);
    }
  }
}