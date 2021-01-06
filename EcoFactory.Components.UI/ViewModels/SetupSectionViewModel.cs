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
using Emporer.WPF.ViewModels;
using Milan.Simulation;
using Milan.Simulation.Factories;
using Milan.Simulation.Resources;
using Milan.Simulation.Resources.Factories;
using Milan.Simulation.Resources.UI.ViewModels;
using Milan.Simulation.UI.ViewModels;

namespace EcoFactory.Components.UI.ViewModels
{
  public sealed class SetupSectionViewModel : Screen
  {
    private const string ProductTypeParam = "productType";
    private const string DistributionParam = "distribution";
    private readonly ObservableCollection<object> _availableProductTypesForSpecificSetupDurations;
    private readonly IDistributionConfigurationViewModelFactory _distributionConfigurationViewModelFactory;
    private readonly IWorkstationBase _model;
    private readonly IProductTypeDistributionFactory _productTypeDistributionFactory;
    private readonly IResourcePoolResourceTypeAmountFactory _resourcesFactory;

    public SetupSectionViewModel(IWorkstationBase model,
                                 IResourcePoolResourceTypeAmountFactory resourcesFactory,
                                 IProductTypeDistributionFactory productTypeDistributionFactory,
                                 IDistributionConfigurationViewModelFactory distributionConfigurationViewModelFactory)
    {
      _model = model;
      _resourcesFactory = resourcesFactory;
      _productTypeDistributionFactory = productTypeDistributionFactory;
      _distributionConfigurationViewModelFactory = distributionConfigurationViewModelFactory;

      DisplayName = "setup";

      SetupDuration = WrapDistribution(() => _model.SetupDuration, v => _model.SetupDuration = v);
      ProductTypeSpecificSetupDurations = WrapProductTypeDistributions(_model.SetupDurations);
      SetupResources = WrapResourcePoolResourceTypeAmounts(_model.SetupResources);
      _availableProductTypesForSpecificSetupDurations =
        new ObservableCollection<object>(GetAvailableProductTypes(ProductTypeSpecificSetupDurations.Select(ptsd => ptsd.ProductType.Model)));
      AddProductTypeSpecificSetupDurationCommand = CreateAddProductTypeSpecificDistributionCommandItem("Add",
                                                                                                       _availableProductTypesForSpecificSetupDurations,
                                                                                                       AddSetupDuration);
      AddSetupResourceCommand = new AddResourceCommand(_model.Model, AddSetupResource, _model.SetupResources);
    }

    public DistributionSelectorViewModel SetupDuration { get; private set; }
    public ObservableCollection<ResourcePoolResourceTypeAmountEditorViewModel> SetupResources { get; private set; }
    public AddResourceCommand AddSetupResourceCommand { get; private set; }
    public ICommand RemoveCommand { get; private set; }
    public ObservableCollection<ProductTypeDistributionEditorViewModel> ProductTypeSpecificSetupDurations { get; private set; }
    public ChainedParameterCommandViewModel AddProductTypeSpecificSetupDurationCommand { get; private set; }

    public bool HasSetup
    {
      get { return _model.HasSetup; }
      set
      {
        if (_model.HasSetup == value)
        {
          return;
        }
        _model.HasSetup = value;
        NotifyOfPropertyChange(() => HasSetup);
      }
    }

    public bool IsDemandingProcessingResourcesInSetup
    {
      get { return _model.IsDemandingProcessingResourcesInSetup; }
      set
      {
        if (_model.IsDemandingProcessingResourcesInSetup == value)
        {
          return;
        }
        _model.IsDemandingProcessingResourcesInSetup = value;
        NotifyOfPropertyChange(() => IsDemandingProcessingResourcesInSetup);
      }
    }

    private void AddSetupResource(IResourcePool resourcePool, IResourceType resourceType)
    {
      AddResourceTypeAmount(resourcePool, resourceType, vm => SetupResources.Add(vm), _model.AddSetupResource);
      AddSetupResourceCommand.Refresh(_model.SetupResources);
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

    private ChainedParameterCommandViewModel CreateAddProductTypeSpecificDistributionCommandItem(string displayText,
                                                                                                    ObservableCollection<object> availableProductTypes,
                                                                                                    Action<IDictionary<string, object>> addAction)
    {
      var productTypeParameterSet = new ParameterSet(ProductTypeParam, "Select a product type", availableProductTypes);

      var distributionsParameterSet = new ParameterSet(DistributionParam,
                                                       "Select a distribution type",
                                                       new ObservableCollection<object>(
                                                                                        _distributionConfigurationViewModelFactory
                                                                                          .GetDescriptorsOfAllAvailableDistributions()
                                                                                          .ToArray()));

      var chainedParameterCommand = new ChainedParameterCommandViewModel(new[]
                                                                            {
                                                                              productTypeParameterSet,
                                                                              distributionsParameterSet
                                                                            },
                                                                            addAction)
                                    {
                                      DisplayText = displayText
                                    };

      return chainedParameterCommand;
    }

    private void AddSetupDuration(IDictionary<string, object> parameters)
    {
      AddProductTypeSpecificDistribution(parameters,
                                         ptDistVm =>
                                         {
                                           ProductTypeSpecificSetupDurations.Add(ptDistVm);
                                           // remove product type from selectable parameter values (so it can't be selected twice)
                                           _availableProductTypesForSpecificSetupDurations.Remove(ptDistVm.ProductType.Model);
                                         },
                                         _model.AddSetup);
    }

    private void RemoveProductTypeDistribution(ProductTypeDistributionEditorViewModel item)
    {
      Utils.TryRemoveItem<ProductTypeDistributionEditorViewModel, IProductTypeDistribution>(item,
                                                                                            ProductTypeSpecificSetupDurations,
                                                                                            RemoveSetupDuration);
    }

    private void RemoveResourceTypeAmount(ResourcePoolResourceTypeAmountEditorViewModel item)
    {
      if (Utils.TryRemoveItem<ResourcePoolResourceTypeAmountEditorViewModel, IResourcePoolResourceTypeAmount>(item,
                                                                                                              SetupResources,
                                                                                                              RemoveSetupResource))
      {
        AddSetupResourceCommand.Refresh(_model.SetupResources);
      }
    }

    private void RemoveSetupDuration(IProductTypeDistribution productTypeDistribution)
    {
      _model.RemoveSetup(productTypeDistribution);
      _availableProductTypesForSpecificSetupDurations.Add(productTypeDistribution.ProductType);
      NotifyOfPropertyChange(() => AddProductTypeSpecificSetupDurationCommand);
    }

    private void RemoveSetupResource(IResourcePoolResourceTypeAmount resourceResourceTypeAmount)
    {
      _model.RemoveSetupResource(resourceResourceTypeAmount);
    }

    private DistributionSelectorViewModel WrapDistribution(Func<IDistributionConfiguration> getValue, Action<IDistributionConfiguration> setValue)
    {
      return new DistributionSelectorViewModel(_distributionConfigurationViewModelFactory,
                                               new PropertyWrapper<IDistributionConfiguration>(getValue, setValue));
    }

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

    private ObservableCollection<ProductTypeDistributionEditorViewModel> WrapProductTypeDistributions(
      IEnumerable<IProductTypeDistribution> productTypeDistributions)
    {
      // create a vm for each PT related distribution
      var vms =
        productTypeDistributions.Select(ptDist => new ProductTypeDistributionEditorViewModel(ptDist, _distributionConfigurationViewModelFactory));
      return new ObservableCollection<ProductTypeDistributionEditorViewModel>(vms);
    }

    private IEnumerable<IProductType> GetAvailableProductTypes(IEnumerable<IProductType> productTypesAlreadyInUse)
    {
      return _model.Model.Entities.OfType<IProductType>()
                   .Except(productTypesAlreadyInUse);
    }
  }
}