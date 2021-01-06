
// Copyright (c) 2013 HTW Berlin
// All rights reserved.


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Emporer.Math.Distribution;
using Emporer.Math.Distribution.UI;
using Emporer.Math.Distribution.UI.Factories;
using Emporer.Math.Distribution.UI.ViewModels;
using Emporer.WPF.ViewModels;
using Milan.Simulation.Factories;

namespace Milan.Simulation.UI.ViewModels
{
  public abstract class StationaryElementEditViewModel : EntityEditViewModel
  {
    private const string ProductTypeParam = "productType";
    private const string DistributionParam = "distribution";
    private const string DestinationParam = "destination";
    private readonly IConnectionFactory _connectionFactory;
    protected readonly IDistributionConfigurationViewModelFactory _distributionConfigurationViewModelFactory;
    private readonly IProductTypeDistributionFactory _productTypeDistributionFactory;
    private readonly IStationaryElement _stationaryElement;

    public StationaryElementEditViewModel(IStationaryElement stationaryElement,
                                          IDistributionConfigurationViewModelFactory distributionConfigurationViewModelFactory,
                                          IProductTypeDistributionFactory productTypeDistributionFactory,
                                          IConnectionFactory connectionFactory,
                                          string name)
      : base(stationaryElement, name)
    {
      _stationaryElement = stationaryElement;
      _distributionConfigurationViewModelFactory = distributionConfigurationViewModelFactory;
      _productTypeDistributionFactory = productTypeDistributionFactory;
      _connectionFactory = connectionFactory;
      Connections = WrapConnections(_stationaryElement.Connections);

      AddConnectionCommand = CreateAddConnectionCommandItem("Add", GetAvailableDestinations, AddConnection);
    }

    public ObservableCollection<ConnectionEditorViewModel> Connections { get; private set; }
    public ChainedParameterCommandViewModel AddConnectionCommand { get; private set; }

    private ChainedParameterCommandViewModel CreateAddConnectionCommandItem(string add,
                                                                               Func<ObservableCollection<object>> getDestinations,
                                                                               Action<IDictionary<string, object>> addConnection)
    {
      var productTypeParameterSet = new ParameterSet(DestinationParam, "Select a destination", getDestinations());

      var chainedParameterCommand = new ChainedParameterCommandViewModel(new[]
                                                                            {
                                                                              productTypeParameterSet
                                                                            },
                                                                            addConnection)
      {
        DisplayText = add
      };

      return chainedParameterCommand;
    }

    protected void AddConnection(IDictionary<string, object> parameters)
    {
      var destination = (IStationaryElement)parameters[DestinationParam];

      var connection = _connectionFactory.Create();
      connection.Destination = destination;
      _stationaryElement.Add(connection);

      var vm = new ConnectionEditorViewModel(connection, GetAvailableProductTypes(connection.ProductTypes));
      Connections.Add(vm);
    }

    protected void RemoveConnection(ConnectionEditorViewModel item)
    {
      Connections.Remove(item);
      var model = (IConnection)item.Model;
      _stationaryElement.Remove(model);
    }

    protected DistributionSelectorViewModel WrapDistribution(Func<IDistributionConfiguration> getValue, Action<IDistributionConfiguration> setValue)
    {
      return new DistributionSelectorViewModel(_distributionConfigurationViewModelFactory,
                                               new PropertyWrapper<IDistributionConfiguration>(getValue, setValue));
    }

    protected IEnumerable<IProductType> GetAvailableProductTypes(IEnumerable<IProductType> productTypesAlreadyInUse)
    {
      return _stationaryElement.Model.Entities.OfType<IProductType>()
                               .Except(productTypesAlreadyInUse);
    }

    protected ObservableCollection<object> GetAvailableDestinations()
    {
      return new ObservableCollection<object>(_stationaryElement.Model.Entities.OfType<IStationaryElement>()
                                                                .Where(
                                                                       se =>
                                                                       se.CanConnectToSource(_stationaryElement) &&
                                                                       _stationaryElement.CanConnectToDestination(se))
                                                                .OrderBy(se => se.Name));
    }

    protected ChainedParameterCommandViewModel CreateAddProductTypeSpecificDistributionCommandItem(string displayText,
                                                                                                      ObservableCollection<object>
                                                                                                        availableProductTypes,
                                                                                                      Action<IDictionary<string, object>> addAction)
    {
      var productTypeParameterSet = new ParameterSet(ProductTypeParam, "Select a product type", availableProductTypes);

      var distributionsParameterSet = new ParameterSet(DistributionParam,
                                                       "Select a distribution type",
                                                       new ObservableCollection<object>(
                                                         _distributionConfigurationViewModelFactory.GetDescriptorsOfAllAvailableDistributions()
                                                                                                   .ToArray()));

      var chainedParameterCommand = new ChainedParameterCommandViewModel(new[]
                                                                            {
                                                                              productTypeParameterSet, distributionsParameterSet
                                                                            },
                                                                            addAction)
      {
        DisplayText = displayText
      };

      return chainedParameterCommand;
    }

    protected void AddProductTypeSpecificDistribution(IDictionary<string, object> parameters,
                                                      Action<ProductTypeDistributionEditorViewModel> updateViewModel,
                                                      Action<IProductTypeDistribution> addToModel)
    {
      // extract product type from parameter set
      var productType = (IProductType)parameters[ProductTypeParam];

      // extract id & create dist cfg from parameter set
      var distDescriptor = (DistributionDescriptor)parameters[DistributionParam];
      var dist = distDescriptor.DistributionFactory.CreateConfiguration();

      // use values to create new entry, add to model (workstation)
      var ptSpecificDuration = _productTypeDistributionFactory.Create();
      ptSpecificDuration.ProductType = productType;
      ptSpecificDuration.DistributionConfiguration = dist;
      addToModel(ptSpecificDuration);

      var vm = new ProductTypeDistributionEditorViewModel(ptSpecificDuration, _distributionConfigurationViewModelFactory);

      updateViewModel(vm);
    }

    protected ObservableCollection<ProductTypeDistributionEditorViewModel> WrapProductTypeDistributions(
      IEnumerable<IProductTypeDistribution> productTypeDistributions)
    {
      // create a vm for each PT related distribution
      var vms =
        productTypeDistributions.Select(ptDist => new ProductTypeDistributionEditorViewModel(ptDist, _distributionConfigurationViewModelFactory));
      return new ObservableCollection<ProductTypeDistributionEditorViewModel>(vms);
    }

    protected ObservableCollection<ConnectionEditorViewModel> WrapConnections(IEnumerable<IConnection> connections)
    {
      // create a vm for each PT related distribution
      var vms = connections.Select(con => new ConnectionEditorViewModel(con, GetAvailableProductTypes(con.ProductTypes)));
      return new ObservableCollection<ConnectionEditorViewModel>(vms);
    }

  }
}