#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using Emporer.WPF.Commands;
using Emporer.WPF.ViewModels;

namespace Milan.Simulation.UI.ViewModels
{
  public class ConnectionEditorViewModel : PropertyChangedBase, IEditViewModel  
  {
    private readonly ObservableCollection<object> _availableProductTypes;
    private readonly IConnection _model;

    public ConnectionEditorViewModel(IConnection model, IEnumerable<IProductType> productTypes)
    {
      _model = model;
      Model = _model;

      _availableProductTypes = new ObservableCollection<object>(productTypes);
      RoutedProductTypes = InitializeProductTypes(model.ProductTypes);

      AddProductTypeCommand = CreateAddProductTypeCommandItem("Add", _availableProductTypes, AddProductType);
      RemoveCommand = new RelayCommand(Remove);
    }

    public ICommand RemoveCommand { get; private set; }

    public ObservableCollection<ProductTypeViewModel> RoutedProductTypes { get; private set; }

    public ChainedParameterCommandViewModel AddProductTypeCommand { get; set; }

    public bool IsRoutingPerProductType
    {
      get { return _model.IsRoutingPerProductType; }
      set
      {
        if (_model.IsRoutingPerProductType == value)
        {
          return;
        }
        _model.IsRoutingPerProductType = value;
        NotifyOfPropertyChange(() => IsRoutingPerProductType);
      }
    }

    public int Priority
    {
      get { return _model.Priority; }
      set
      {
        if (_model.Priority == value)
        {
          return;
        }
        _model.Priority = value;
        NotifyOfPropertyChange(() => Priority);
      }
    }

    public IStationaryElement Destination
    {
      get { return _model.Destination; }
      set
      {
        if (_model.Destination == value)
        {
          return;
        }
        _model.Destination = value;
        NotifyOfPropertyChange(() => Destination);
      }
    }
    
    public void Remove(object item)
    {
      var pt = item as ProductTypeViewModel;

      if (pt == null)
      {
        return;
      }

      RemoveProductType(pt);
    }

    private ChainedParameterCommandViewModel CreateAddProductTypeCommandItem(string displayText,
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

    private ObservableCollection<ProductTypeViewModel> InitializeProductTypes(IEnumerable<IProductType> productTypeAmounts)
    {
      // create a vm for each PT related distribution
      var vms = productTypeAmounts.Select(ptDist => new ProductTypeViewModel(ptDist));
      return new ObservableCollection<ProductTypeViewModel>(vms);
    }

    private void AddProductType(IDictionary<string, object> parameters)
    {
      AddProductTypeSpecificCapacity(parameters,
                                     vm =>
                                     {
                                       RoutedProductTypes.Add(vm);
                                       // remove product type from selectable parameter values (so it can't be selected twice)
                                       _availableProductTypes.Remove(vm.Model);
                                     },
                                     _model.Add);
    }

    private void AddProductTypeSpecificCapacity(IDictionary<string, object> parameters,
                                                Action<ProductTypeViewModel> updateViewModel,
                                                Action<IProductType> addToModel)
    {
      // extract product type from parameter set
      var productType = (IProductType) parameters["productType"];

      addToModel(productType);

      var vm = new ProductTypeViewModel(productType);

      updateViewModel(vm);
    }

    
    private void RemoveProductType(ProductTypeViewModel item)
    {
      RoutedProductTypes.Remove(item);
      _model.Remove(item.Model);
      _availableProductTypes.Add(item.Model);
      NotifyOfPropertyChange(() => AddProductTypeCommand);
    }

    public string DisplayName { get; set; }
    public object Model { get; private set; }
  }
}