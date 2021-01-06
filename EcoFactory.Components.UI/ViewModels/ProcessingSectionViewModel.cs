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
using Emporer.Math.Distribution.UI;
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
  public sealed class ProcessingSectionViewModel : Screen
  {
    private const string ProductTypeParam = "productType";
    private const string DistributionParam = "distribution";

    private readonly ObservableCollection<object> _availableProductTypesForSpecificBatchSizes;
    private readonly ObservableCollection<object> _availableProductTypesForSpecificProcessingDurations;
    private readonly IDistributionConfigurationViewModelFactory _distributionConfigurationViewModelFactory;
    private readonly IWorkstationBase _model;
    private readonly IProductTypeDistributionFactory _productTypeDistributionFactory;
    private readonly IProductTypeSpecificResourceFactory _productTypeSpecificResourcesFactory;
    private readonly IResourcePoolResourceTypeAmountFactory _resourcesFactory;

    public ProcessingSectionViewModel(IWorkstationBase model,
                                      IResourcePoolResourceTypeAmountFactory resourcesFactory,
                                      IProductTypeSpecificResourceFactory productTypeSpecificResourcesFactory,
                                      IProductTypeDistributionFactory productTypeDistributionFactory,
                                      IDistributionConfigurationViewModelFactory distributionConfigurationViewModelFactory,
                                      IDistributionSelectionViewModelFactory distributionSelectionViewModelFactory,
                                      IChainedParameterCommandFactory chainedParameterCommandFactory)
    {
      _model = model;
      _resourcesFactory = resourcesFactory;
      _productTypeSpecificResourcesFactory = productTypeSpecificResourcesFactory;
      _productTypeDistributionFactory = productTypeDistributionFactory;
      _distributionConfigurationViewModelFactory = distributionConfigurationViewModelFactory;

      DisplayName = "processing";

      BatchSize =
        distributionSelectionViewModelFactory.Create(new PropertyWrapper<IDistributionConfiguration>(() => _model.BatchSize, v => _model.BatchSize = v));
      ProcessingDuration =
        distributionSelectionViewModelFactory.Create(new PropertyWrapper<IDistributionConfiguration>(() => _model.ProcessingDuration,
                                                                                                     v => _model.ProcessingDuration = v));

      ProductTypeSpecificProcessingDurations = WrapProductTypeDistributions(_model.ProcessingDurations);
      ProductTypeSpecificBatchSizes = WrapProductTypeDistributions(_model.BatchSizes);

      ProcessingResources = WrapResourcePoolResourceTypeAmounts(_model.ProcessingResources);
      ProductTypeSpecificProcessingResources = WrapProductTypeSpecificResourcePoolResourceTypeAmounts(_model.ProductTypeSpecificProcessingResources);

      // if distributions are specified per product type, the available one need to be tracked
      // (else a pt could be added twice)
      _availableProductTypesForSpecificProcessingDurations =
        new ObservableCollection<object>(_model.GetAvailableProductTypes(ProductTypeSpecificProcessingDurations.Select(ptsd => ptsd.ProductType.Model)));

      _availableProductTypesForSpecificBatchSizes =
        new ObservableCollection<object>(_model.GetAvailableProductTypes(ProductTypeSpecificBatchSizes.Select(ptsd => ptsd.ProductType.Model)));

      AddProductTypeSpecificProcessingDurationCommand = chainedParameterCommandFactory.CreateAddProductTypeSpecificDistributionCommand("Add",
                                                                                                                                       _availableProductTypesForSpecificProcessingDurations,
                                                                                                                                       AddProcessingDuration);
      AddProductTypeSpecificBatchSizeCommand = chainedParameterCommandFactory.CreateAddProductTypeSpecificDistributionCommand("Add",
                                                                                                                              _availableProductTypesForSpecificBatchSizes,
                                                                                                                              AddBatchSize);

      AddProcessingResourceCommand = new AddResourceCommand(_model.Model, AddProcessingResource, _model.ProcessingResources);

      AddProductTypeSpecificProcessingResourceCommand = new AddProductTypeSpecificResourceCommand(_model.Model,
                                                                                                  AddProcessingResource,
                                                                                                  _model.ProductTypeSpecificProcessingResources);
      RemoveCommand = new RelayCommand(Remove);
    }

    public DistributionSelectorViewModel BatchSize { get; private set; }
    public DistributionSelectorViewModel ProcessingDuration { get; private set; }

    public ObservableCollection<ProductTypeDistributionEditorViewModel> ProductTypeSpecificProcessingDurations { get; private set; }

    public ChainedParameterCommandViewModel AddProductTypeSpecificProcessingDurationCommand { get; private set; }

    public ObservableCollection<ProductTypeDistributionEditorViewModel> ProductTypeSpecificBatchSizes { get; private set; }

    public ChainedParameterCommandViewModel AddProductTypeSpecificBatchSizeCommand { get; private set; }
    public ObservableCollection<ResourcePoolResourceTypeAmountEditorViewModel> ProcessingResources { get; private set; }
    public AddResourceCommand AddProcessingResourceCommand { get; private set; }

    public ObservableCollection<ProductTypeSpecificResourceEditorViewModel> ProductTypeSpecificProcessingResources { get; private set; }

    public AddProductTypeSpecificResourceCommand AddProductTypeSpecificProcessingResourceCommand { get; private set; }
    public ICommand RemoveCommand { get; private set; }

    private ObservableCollection<ProductTypeSpecificResourceEditorViewModel> WrapProductTypeSpecificResourcePoolResourceTypeAmounts(
      IEnumerable<IProductTypeSpecificResource> productTypeSpecificProcessingResources)
    {
      //todo: use factory
      var vms = productTypeSpecificProcessingResources.Select(p => new ProductTypeSpecificResourceEditorViewModel(p))
                                                      .ToList();

      return new ObservableCollection<ProductTypeSpecificResourceEditorViewModel>(vms);
    }

    private ObservableCollection<ResourcePoolResourceTypeAmountEditorViewModel> WrapResourcePoolResourceTypeAmounts(
      IEnumerable<IResourcePoolResourceTypeAmount> processingResources)
    {
      //todo: use factory
      var vms = processingResources.Select(rta => new ResourcePoolResourceTypeAmountEditorViewModel(rta));
      return new ObservableCollection<ResourcePoolResourceTypeAmountEditorViewModel>(vms);
    }

    private void AddProcessingResource(IResourcePool resourcePool, IResourceType resourceType)
    {
      AddResourceTypeAmount(resourcePool, resourceType, vm => ProcessingResources.Add(vm), _model.AddProcessingResource);
      AddProcessingResourceCommand.Refresh(_model.ProcessingResources);
    }

    private void AddResourceTypeAmount(IResourcePool resourcePool,
                                       IResourceType resourceType,
                                       Action<ResourcePoolResourceTypeAmountEditorViewModel> addViewModel,
                                       Action<IResourcePoolResourceTypeAmount> addToModel)
    {
      var resource = _resourcesFactory.Create();
      resource.ResourceType = resourceType;
      resource.ResourcePool = resourcePool;
      resource.Amount = 1;
      //todo: use factory
      addViewModel(new ResourcePoolResourceTypeAmountEditorViewModel(resource));
      addToModel(resource);
    }

    private void AddProcessingResource(IProductType productType, IResourcePool resourcePool, IResourceType resourceType)
    {
      var resource = _productTypeSpecificResourcesFactory.Create();
      resource.ProductType = productType;
      resource.ResourceType = resourceType;
      resource.ResourcePool = resourcePool;
      resource.Amount = 1;
      //todo: use factory
      ProductTypeSpecificProcessingResources.Add(new ProductTypeSpecificResourceEditorViewModel(resource));
      _model.AddProcessingResource(resource);
      AddProductTypeSpecificProcessingResourceCommand.Refresh(_model.ProductTypeSpecificProcessingResources);
    }

    private void AddProcessingDuration(IDictionary<string, object> parameters)
    {
      AddProductTypeSpecificDistribution(parameters,
                                         vm =>
                                         {
                                           ProductTypeSpecificProcessingDurations.Add(vm);
                                           // remove product type from selectable parameter values (so it can't be selected twice)
                                           _availableProductTypesForSpecificProcessingDurations.Remove(vm.ProductType.Model);
                                         },
                                         _model.AddProcessing);
    }

    private void AddBatchSize(IDictionary<string, object> parameters)
    {
      AddProductTypeSpecificDistribution(parameters,
                                         vm =>
                                         {
                                           ProductTypeSpecificBatchSizes.Add(vm);
                                           // remove product type from selectable parameter values (so it can't be selected twice)
                                           _availableProductTypesForSpecificBatchSizes.Remove(vm.ProductType.Model);
                                         },
                                         _model.AddBatchSize);
    }

    private void Remove(object item)
    {
      if (item == null)
      {
        return;
      }
      if (item is ProductTypeDistributionEditorViewModel)
      {
        RemoveProductTypeDistribution((ProductTypeDistributionEditorViewModel) item);
        return;
      }
      if (item is ResourcePoolResourceTypeAmountEditorViewModel)
      {
        RemoveResourceTypeAmount((ResourcePoolResourceTypeAmountEditorViewModel) item);
        return;
      }
      if (item is ProductTypeSpecificResourceEditorViewModel)
      {
        RemoveResourceTypeAmount((ProductTypeSpecificResourceEditorViewModel) item);
      }
    }

    private void RemoveResourceTypeAmount(ProductTypeSpecificResourceEditorViewModel item)
    {
      if (Utils.TryRemoveItem<ProductTypeSpecificResourceEditorViewModel, IProductTypeSpecificResource>(item,
                                                                                                        ProductTypeSpecificProcessingResources,
                                                                                                        _model.RemoveProcessingResource))
      {
        AddProductTypeSpecificProcessingResourceCommand.Refresh(_model.ProductTypeSpecificProcessingResources);
      }
    }

    private void RemoveProductTypeDistribution(ProductTypeDistributionEditorViewModel item)
    {
      if (Utils.TryRemoveItem<ProductTypeDistributionEditorViewModel, IProductTypeDistribution>(item,
                                                                                                ProductTypeSpecificProcessingDurations,
                                                                                                RemoveProcessingDuration))
      {
      }
      else if (Utils.TryRemoveItem<ProductTypeDistributionEditorViewModel, IProductTypeDistribution>(item, ProductTypeSpecificBatchSizes, RemoveBatchSize))
      {
      }
    }

    private void RemoveResourceTypeAmount(ResourcePoolResourceTypeAmountEditorViewModel item)
    {
      if (Utils.TryRemoveItem<ResourcePoolResourceTypeAmountEditorViewModel, IResourcePoolResourceTypeAmount>(item,
                                                                                                              ProcessingResources,
                                                                                                              RemoveProcessingResource))
      {
        AddProcessingResourceCommand.Refresh(_model.ProcessingResources);
      }
    }

    private void RemoveProcessingDuration(IProductTypeDistribution productTypeDistribution)
    {
      _model.RemoveProcessing(productTypeDistribution);
      _availableProductTypesForSpecificProcessingDurations.Add(productTypeDistribution.ProductType);
      NotifyOfPropertyChange(() => AddProductTypeSpecificProcessingDurationCommand);
    }

    private void RemoveBatchSize(IProductTypeDistribution productTypeDistribution)
    {
      _model.RemoveBatchSize(productTypeDistribution);
      _availableProductTypesForSpecificBatchSizes.Add(productTypeDistribution.ProductType);
      NotifyOfPropertyChange(() => AddProductTypeSpecificBatchSizeCommand);
    }

    private void RemoveProcessingResource(IResourcePoolResourceTypeAmount resourceResourceTypeAmount)
    {
      _model.RemoveProcessingResource(resourceResourceTypeAmount);
    }

    private ObservableCollection<ProductTypeDistributionEditorViewModel> WrapProductTypeDistributions(
      IEnumerable<IProductTypeDistribution> productTypeDistributions)
    {
      // create a vm for each PT related distribution
      var vms =
        productTypeDistributions.Select(ptDist => new ProductTypeDistributionEditorViewModel(ptDist, _distributionConfigurationViewModelFactory));
      return new ObservableCollection<ProductTypeDistributionEditorViewModel>(vms);
    }

    private void AddProductTypeSpecificDistribution(IDictionary<string, object> parameters,
                                                    Action<ProductTypeDistributionEditorViewModel> updateViewModel,
                                                    Action<IProductTypeDistribution> addToModel)
    {
      // extract product type from parameter set
      var productType = (IProductType) parameters[ProductTypeParam];

      // extract id & create dist cfg from parameter set
      var distDescriptor = (DistributionDescriptor) parameters[DistributionParam];
      var dist = distDescriptor.DistributionFactory.CreateConfiguration();

      // use values to create new entry, add to model (workstation)
      var ptSpecificDuration = _productTypeDistributionFactory.Create();
      ptSpecificDuration.ProductType = productType;
      ptSpecificDuration.DistributionConfiguration = dist;
      addToModel(ptSpecificDuration);

      var vm = new ProductTypeDistributionEditorViewModel(ptSpecificDuration, _distributionConfigurationViewModelFactory);

      updateViewModel(vm);
    }
  }
}