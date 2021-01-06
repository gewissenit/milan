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
using Milan.Simulation.UI.Factories;
using Milan.Simulation.UI.ViewModels;

namespace EcoFactory.Components.UI.ViewModels
{
  public sealed class ArrivalsSectionViewModel : Screen
  {
    private const string ProductTypeParam = "productType";
    private const string DistributionParam = "distribution";

    private readonly ObservableCollection<object> _availableProductTypesForArrivals;
    private readonly ObservableCollection<object> _availableProductTypesForSpecificBatchSizes;
    private readonly IDistributionConfigurationViewModelFactory _distributionConfigurationViewModelFactory;
    private readonly IEntryPoint _model;
    private readonly IProductTypeDistributionFactory _productTypeDistributionFactory;

    public ArrivalsSectionViewModel(IEntryPoint model,
                                    IDistributionConfigurationViewModelFactory distributionConfigurationViewModelFactory,
                                    IProductTypeDistributionFactory productTypeDistributionFactory,
                                    IDistributionSelectionViewModelFactory distributionSelectionViewModelFactory,
                                    IChainedParameterCommandFactory chainedParameterCommandFactory)
    {
      DisplayName = "arrivals";
      _model = model;
      _distributionConfigurationViewModelFactory = distributionConfigurationViewModelFactory;
      _productTypeDistributionFactory = productTypeDistributionFactory;
      BatchSize =
        distributionSelectionViewModelFactory.Create(new PropertyWrapper<IDistributionConfiguration>(() => _model.BatchSize, v => _model.BatchSize = v));

      // initialize complex (dictionary-like) properties
      ProductTypeSpecificArrivals = WrapProductTypeDistributions(_model.ArrivalOccurrences);
      ProductTypeSpecificBatchSizes = WrapProductTypeDistributions(_model.BatchSizes);

      _availableProductTypesForSpecificBatchSizes =
        new ObservableCollection<object>(_model.GetAvailableProductTypes(_model.BatchSizes.Select(x => x.ProductType)));

      _availableProductTypesForArrivals =
        new ObservableCollection<object>(_model.GetAvailableProductTypes(_model.ArrivalOccurrences.Select(x => x.ProductType)));

      AddProductTypeSpecificArrivalCommand = chainedParameterCommandFactory.CreateAddProductTypeSpecificDistributionCommand("Add",
                                                                                                                            _availableProductTypesForArrivals,
                                                                                                                            AddArrivalOccurence);

      AddProductTypeSpecificBatchSizeCommand = chainedParameterCommandFactory.CreateAddProductTypeSpecificDistributionCommand("Add",
                                                                                                                              _availableProductTypesForSpecificBatchSizes,
                                                                                                                              AddBatchSize);
      RemoveCommand = new RelayCommand(Remove);
    }

    public ObservableCollection<ProductTypeDistributionEditorViewModel> ProductTypeSpecificArrivals { get; set; }
    public ChainedParameterCommandViewModel AddProductTypeSpecificArrivalCommand { get; set; }
    public ObservableCollection<ProductTypeDistributionEditorViewModel> ProductTypeSpecificBatchSizes { get; set; }
    public ChainedParameterCommandViewModel AddProductTypeSpecificBatchSizeCommand { get; set; }
    public DistributionSelectorViewModel Arrivals { get; set; }
    public DistributionSelectorViewModel BatchSize { get; set; }
    public ICommand RemoveCommand { get; private set; }

    private void AddArrivalOccurence(IDictionary<string, object> parameters)
    {
      AddProductTypeSpecificDistribution(parameters,
                                         ptDistVm =>
                                         {
                                           ProductTypeSpecificArrivals.Add(ptDistVm);
                                           // remove product type from selectable parameter values (so it can't be selected twice)
                                           _availableProductTypesForArrivals.Remove(ptDistVm.ProductType.Model);
                                         },
                                         _model.AddArrival);
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

    public void Remove(object item)
    {
      var ptdist = item as ProductTypeDistributionEditorViewModel;

      if (Utils.TryRemoveItem<ProductTypeDistributionEditorViewModel, IProductTypeDistribution>(ptdist, ProductTypeSpecificArrivals, RemoveArrival))
      {
        return;
      }
      if (Utils.TryRemoveItem<ProductTypeDistributionEditorViewModel, IProductTypeDistribution>(ptdist, ProductTypeSpecificBatchSizes, RemoveBatchSize))
      {
      }
    }

    private void RemoveArrival(IProductTypeDistribution productTypeDistribution)
    {
      _model.RemoveArrival(productTypeDistribution);
      _availableProductTypesForArrivals.Add(productTypeDistribution.ProductType);
    }

    private void RemoveBatchSize(IProductTypeDistribution productTypeDistribution)
    {
      _model.RemoveBatchSize(productTypeDistribution);
      _availableProductTypesForSpecificBatchSizes.Add(productTypeDistribution.ProductType);
    }

    private ObservableCollection<ProductTypeDistributionEditorViewModel> WrapProductTypeDistributions(
      IEnumerable<IProductTypeDistribution> productTypeDistributions)
    {
      // create a vm for each PT related distribution
      var vms =
        productTypeDistributions.Select(ptDist => new ProductTypeDistributionEditorViewModel(ptDist, _distributionConfigurationViewModelFactory));
      return new ObservableCollection<ProductTypeDistributionEditorViewModel>(vms);
    }
  }
}