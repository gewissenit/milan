using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using EcoFactory.Components.Factories;
using Emporer.Math.Distribution.UI.Factories;
using Emporer.WPF.Commands;
using GeWISSEN.Utils;
using Milan.Simulation.Factories;
using Milan.Simulation.Resources;
using Milan.Simulation.Resources.Factories;
using Milan.Simulation.Resources.UI.ViewModels;
using Milan.Simulation.UI.Factories;

namespace EcoFactory.Components.UI.ViewModels
{
  public sealed class TransformationsSectionViewModel : Screen
  {
    private readonly IAssemblyStation _model;
    private readonly ITransformationRuleFactory _transformationRuleFactory;
    private readonly ITransformationRuleOutputFactory _transformationRuleOutputFactory;
    private readonly IDistributionConfigurationViewModelFactory _distributionConfigurationViewModelFactory;
    private readonly IProductTypeAmountFactory _productTypeAmountFactory;
    private readonly IResourcePoolResourceTypeAmountFactory _resourceFactory;
    private readonly IChainedParameterCommandFactory _chainedParameterCommandFactory;

    public TransformationsSectionViewModel(IAssemblyStation model,
                                           IProductTypeAmountFactory productTypeAmountFactory,
                                           ITransformationRuleOutputFactory transformationRuleOutputFactory,
                                           IDistributionConfigurationViewModelFactory distributionConfigurationViewModelFactory,
                                           IResourcePoolResourceTypeAmountFactory resourceFactory,
                                           IChainedParameterCommandFactory chainedParameterCommandFactory,
                                           ITransformationRuleFactory transformationRuleFactory)
    {
      DisplayName = "transformation";
      _model = model;
      _productTypeAmountFactory = productTypeAmountFactory;
      _transformationRuleOutputFactory = transformationRuleOutputFactory;
      _distributionConfigurationViewModelFactory = distributionConfigurationViewModelFactory;
      _resourceFactory = resourceFactory;
      _chainedParameterCommandFactory = chainedParameterCommandFactory;
      _transformationRuleFactory = transformationRuleFactory;

      TransformationRules = new ObservableCollection<TransformationRuleEditorViewModel>(_model.TransformationRules.Select(CreateRule));
      ProcessingResources = InitializeResourcePoolResourceTypeAmounts(_model.ProcessingResources);
      AddRuleCommand = new RelayCommand(AddRule);
      AddProcessingResourceCommand = new AddResourceCommand(_model.Model,
                                                            AddProcessingResource,
                                                            _model.ProcessingResources);
      RemoveCommand = new RelayCommand(Remove);
    }

    public ObservableCollection<TransformationRuleEditorViewModel> TransformationRules { get; private set; }
    public ObservableCollection<ResourcePoolResourceTypeAmountEditorViewModel> ProcessingResources { get; set; }
    public ICommand AddRuleCommand { get; private set; }
    public AddResourceCommand AddProcessingResourceCommand { get; set; }
    public ICommand RemoveCommand { get; private set; }


    private TransformationRuleEditorViewModel CreateRule(ITransformationRule rule)
    {
      //todo: use factory
      return new TransformationRuleEditorViewModel(rule,
                                                   _model.Model,
                                                   _productTypeAmountFactory,
                                                   _transformationRuleOutputFactory,
                                                   _distributionConfigurationViewModelFactory,
                                                   _resourceFactory,
                                                   _chainedParameterCommandFactory,
                                                   false);
    }

    private ObservableCollection<ResourcePoolResourceTypeAmountEditorViewModel> InitializeResourcePoolResourceTypeAmounts(IEnumerable<IResourcePoolResourceTypeAmount> resources)
    {
      var vms = resources.Select(rta => new ResourcePoolResourceTypeAmountEditorViewModel(rta));
      return new ObservableCollection<ResourcePoolResourceTypeAmountEditorViewModel>(vms);
    }

    private void AddProcessingResource(IResourcePool resourcePool,
                                       IResourceType resourceType)
    {
      AddResourceTypeAmount(resourcePool,
                            resourceType,
                            vm => ProcessingResources.Add(vm),
                            _model.AddProcessingResource);
      AddProcessingResourceCommand.Refresh(_model.ProcessingResources);
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

    private void RemoveProcessingResource(IResourcePoolResourceTypeAmount resourceResourceTypeAmount)
    {
      _model.RemoveProcessingResource(resourceResourceTypeAmount);
    }

    private void Remove(object item)
    {
      new Map<Type, Action<object>>()
      {
        {
          typeof(TransformationRuleEditorViewModel), RemoveRule
        },
        {
          typeof(ResourcePoolResourceTypeAmountEditorViewModel), RemoveResource
        }
      }[item.GetType()](item);
    }

    private void RemoveResource(object item)
    {
      var resourceTypeAmount = (ResourcePoolResourceTypeAmountEditorViewModel) item;
      if (Utils.TryRemoveItem<ResourcePoolResourceTypeAmountEditorViewModel, IResourcePoolResourceTypeAmount>(resourceTypeAmount,
                                                                                                              ProcessingResources,
                                                                                                              RemoveProcessingResource))
      {
        AddProcessingResourceCommand.Refresh(_model.ProcessingResources);
      }
    }

    private void RemoveRule(object obj)
    {
      var rule = ((TransformationRuleEditorViewModel) obj);
      _model.RemoveTransformationRule(rule.Model);
      TransformationRules.Remove(rule);
    }

    private void AddRule(object obj)
    {
      var newRule = _transformationRuleFactory.Create();
      _model.AddTransformationRule(newRule);
      TransformationRules.Add(CreateRule(newRule));
    }
  }
}