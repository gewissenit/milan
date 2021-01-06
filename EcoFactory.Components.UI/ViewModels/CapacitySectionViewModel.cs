using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using Emporer.WPF.Commands;
using Emporer.WPF.ViewModels;
using Milan.Simulation;
using Milan.Simulation.Factories;
using Milan.Simulation.UI.Factories;
using Milan.Simulation.UI.ViewModels;

namespace EcoFactory.Components.UI.ViewModels
{
  internal class CapacitySectionViewModel : Screen
  {
    private readonly IStorage _model;
    private readonly IProductTypeAmountFactory _productTypeAmountFactory;
    private readonly IChainedParameterCommandFactory _chainedParameterCommandFactory;
    private readonly ObservableCollection<object> _availableProductTypesForSpecificCapacities;

    public CapacitySectionViewModel(IStorage model,
                                    IProductTypeAmountFactory productTypeAmountFactory,
                                    IChainedParameterCommandFactory chainedParameterCommandFactory)
    {
      DisplayName = "capacity";

      _model = model;
      _productTypeAmountFactory = productTypeAmountFactory;
      _chainedParameterCommandFactory = chainedParameterCommandFactory;
      _productTypeAmountFactory = productTypeAmountFactory;

      ProductTypeSpecificCapacities = InitializeProductTypeRelatedCapacities(model.ProductTypeCapacities);

      _availableProductTypesForSpecificCapacities =
        new ObservableCollection<object>(_model.GetAvailableProductTypes(ProductTypeSpecificCapacities.Select(ptsd => ptsd.ProductType.Model)));

      AddProductTypeSpecificCapacityCommand = chainedParameterCommandFactory.CreateAddProductTypeSpecificAmountCommand("Add",
                                                                                                                       _availableProductTypesForSpecificCapacities,
                                                                                                                       AddCapacity);
      RemoveCommand = new RelayCommand(Remove);
    }

    public ICommand RemoveCommand { get; private set; }
    public ObservableCollection<ProductTypeAmountEditorViewModel> ProductTypeSpecificCapacities { get; set; }
    public ChainedParameterCommandViewModel AddProductTypeSpecificCapacityCommand { get; set; }

    public bool HasLimitedCapacity
    {
      get { return _model.HasLimitedCapacity; }
      set
      {
        if (_model.HasLimitedCapacity == value)
        {
          return;
        }
        _model.HasLimitedCapacity = value;
        NotifyOfPropertyChange(() => HasLimitedCapacity);
      }
    }

    public int Capacity
    {
      get { return _model.Capacity; }
      set
      {
        if (_model.Capacity == value)
        {
          return;
        }
        _model.Capacity = value;
        NotifyOfPropertyChange(() => Capacity);
      }
    }

    private void Remove(object item)
    {
      var pt = item as ProductTypeAmountEditorViewModel;

      if (pt != null)
      {
        RemoveCapacity(pt);
      }
    }

    private void RemoveCapacity(ProductTypeAmountEditorViewModel pt)
    {
      ProductTypeSpecificCapacities.Remove(pt);
      _model.RemoveProductTypeCapacity(pt.Model);
      _availableProductTypesForSpecificCapacities.Add(pt.Model.ProductType);
      NotifyOfPropertyChange(() => AddProductTypeSpecificCapacityCommand);
    }

    private ObservableCollection<ProductTypeAmountEditorViewModel> InitializeProductTypeRelatedCapacities(
      IEnumerable<IProductTypeAmount> productTypeAmounts)
    {
      // create a vm for each PT related distribution
      //todo: use factory
      return
        new ObservableCollection<ProductTypeAmountEditorViewModel>(productTypeAmounts.Select(ptDist => new ProductTypeAmountEditorViewModel(ptDist)));
    }

    private void AddCapacity(IDictionary<string, object> parameters)
    {
      // extract product type from parameter set
      var productType = (IProductType) parameters["productType"];

      // use values to create new entry, add to model (workstation)
      var ptSpecific = _productTypeAmountFactory.Create();
      ptSpecific.ProductType = productType;
      _model.AddProductTypeCapacity(ptSpecific);
      //todo: use factory
      var vm = new ProductTypeAmountEditorViewModel(ptSpecific);

      ProductTypeSpecificCapacities.Add(vm);
      // remove product type from selectable parameter values (so it can't be selected twice)
      _availableProductTypesForSpecificCapacities.Remove(vm.ProductType.Model);
    }
  }
}