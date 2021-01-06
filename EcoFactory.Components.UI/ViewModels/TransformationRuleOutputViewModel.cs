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
using Emporer.Math.Distribution;
using Emporer.Math.Distribution.UI.Factories;
using Emporer.Math.Distribution.UI.ViewModels;
using Emporer.WPF.Commands;
using Emporer.WPF.ViewModels;
using Milan.Simulation;
using Milan.Simulation.Factories;
using Milan.Simulation.Resources;
using Milan.Simulation.Resources.Factories;
using Milan.Simulation.Resources.UI.ViewModels;
using Milan.Simulation.UI.Factories;
using Milan.Simulation.UI.ViewModels;

namespace EcoFactory.Components.UI.ViewModels
{
  public class TransformationRuleOutputViewModel : PropertyChangedBase
  {
    private readonly ObservableCollection<object> _availableProductTypesForOutput;
    private readonly IModel _owningModel;
    private readonly IProductTypeAmountFactory _productTypeAmountFactory;
    private readonly IResourcePoolResourceTypeAmountFactory _resourceFactory;

    public TransformationRuleOutputViewModel(ITransformationRuleOutput output,
                                             IModel owningModel,
                                             IProductTypeAmountFactory productTypeAmountFactory,
                                             IDistributionConfigurationViewModelFactory distributionConfigurationViewModelFactory,
                                             IResourcePoolResourceTypeAmountFactory resourceFactory,
                                             IChainedParameterCommandFactory chainedParameterCommandFactory)
    {
      Model = output;
      _owningModel = owningModel;
      _productTypeAmountFactory = productTypeAmountFactory;
      _resourceFactory = resourceFactory;

      ProcessingDuration = new DistributionSelectorViewModel(distributionConfigurationViewModelFactory,
                                                             new PropertyWrapper<IDistributionConfiguration>(() => Model.ProcessingDuration,
                                                                                                             v =>
                                                                                                               Model.ProcessingDuration =
                                                                                                                 (DistributionConfiguration) v));

      RemoveCommand = new RelayCommand(Remove, o => true);
      Outputs =
        new ObservableCollection<ProductTypeAmountEditorViewModel>(output.Outputs.Select(ptDist => new ProductTypeAmountEditorViewModel(ptDist)));
      _availableProductTypesForOutput = new ObservableCollection<object>(GetAvailableProductTypes(Outputs.Select(ptsd => ptsd.ProductType.Model)));
      AddOutputCommand = chainedParameterCommandFactory.CreateAddProductTypeSpecificAmountCommand("Add",
                                                                                                  _availableProductTypesForOutput,
                                                                                                  AddProductTypeAsInput);

      Resources = InitializeResourcePoolResourceTypeAmounts(Model.Resources);
      AddResourceCommand = new AddResourceCommand(_owningModel, AddResource, Model.Resources);
    }

    public ICommand RemoveCommand { get; private set; }

    public ChainedParameterCommandViewModel AddOutputCommand { get; set; }
    public ObservableCollection<ResourcePoolResourceTypeAmountEditorViewModel> Resources { get; set; }
    public AddResourceCommand AddResourceCommand { get; set; }

    public ITransformationRuleOutput Model { get; }

    public int Probability
    {
      get { return Model.Probability; }
      set
      {
        Model.Probability = value;
        NotifyOfPropertyChange(() => Probability);
      }
    }

    public DistributionSelectorViewModel ProcessingDuration { get; set; }

    public ObservableCollection<ProductTypeAmountEditorViewModel> Outputs { get; private set; }

    private void Remove(object item)
    {
      var resourceTypeAmount = item as ResourcePoolResourceTypeAmountEditorViewModel;

      if (Utils.TryRemoveItem<ResourcePoolResourceTypeAmountEditorViewModel, IResourcePoolResourceTypeAmount>(resourceTypeAmount,
                                                                                                              Resources,
                                                                                                              RemoveResource))
      {
        AddResourceCommand.Refresh(Model.Resources);
      }
      else if (item is ProductTypeAmountEditorViewModel)
      {
        RemoveOutput((ProductTypeAmountEditorViewModel) item);
      }
    }

    private ObservableCollection<ResourcePoolResourceTypeAmountEditorViewModel> InitializeResourcePoolResourceTypeAmounts(
      IEnumerable<IResourcePoolResourceTypeAmount> resources)
    {
      var vms = resources.Select(rta => new ResourcePoolResourceTypeAmountEditorViewModel(rta));
      return new ObservableCollection<ResourcePoolResourceTypeAmountEditorViewModel>(vms);
    }

    private IEnumerable<IProductType> GetAvailableProductTypes(IEnumerable<IProductType> productTypesAlreadyInUse)
    {
      return _owningModel.Entities.OfType<IProductType>()
                         .Except(productTypesAlreadyInUse);
    }

    private void AddResource(IResourcePool resourcePool, IResourceType resourceType)
    {
      AddResourceTypeAmount(resourcePool, resourceType, vm => Resources.Add(vm), Model.Add);
      AddResourceCommand.Refresh(Model.Resources);
    }

    private void AddResourceTypeAmount(IResourcePool resourcePool,
                                       IResourceType resourceType,
                                       Action<ResourcePoolResourceTypeAmountEditorViewModel> addViewModel,
                                       Action<IResourcePoolResourceTypeAmount> addToModel)
    {
      var resource = _resourceFactory.Create();
      resource.ResourceType = resourceType;
      resource.ResourcePool = resourcePool;
      resource.Amount = 1;
      //todo: use factory
      addViewModel(new ResourcePoolResourceTypeAmountEditorViewModel(resource));
      addToModel(resource);
    }

    private void RemoveResource(IResourcePoolResourceTypeAmount resourceResourceTypeAmount)
    {
      Model.Remove(resourceResourceTypeAmount);
    }

    private void AddProductTypeAsInput(IDictionary<string, object> parameters)
    {
      AddInput(parameters,
               vm =>
               {
                 Outputs.Add(vm);
                 // remove product type from selectable parameter values (so it can't be selected twice)
                 _availableProductTypesForOutput.Remove(vm.ProductType.Model);
               },
               Model.Add);
    }

    private void AddInput(IDictionary<string, object> parameters,
                          Action<ProductTypeAmountEditorViewModel> updateViewModel,
                          Action<IProductTypeAmount> addToModel)
    {
      // extract product type from parameter set
      var productType = (IProductType) parameters["productType"];

      // use values to create new entry, add to model (workstation)
      var ptSpecific = _productTypeAmountFactory.Create();
      ptSpecific.ProductType = productType;
      addToModel(ptSpecific);
      //todo: use factory
      var vm = new ProductTypeAmountEditorViewModel(ptSpecific);

      updateViewModel(vm);
    }

    private void RemoveOutput(ProductTypeAmountEditorViewModel pta)
    {
      Model.Remove(pta.Model);
      _availableProductTypesForOutput.Add(pta.Model);
      Outputs.Remove(pta);
    }
  }
}