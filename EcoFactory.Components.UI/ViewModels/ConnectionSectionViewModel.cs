#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using Emporer.WPF.Commands;
using Emporer.WPF.ViewModels;
using Milan.Simulation;
using Milan.Simulation.Factories;
using Milan.Simulation.UI.ViewModels;

namespace EcoFactory.Components.UI.ViewModels
{
  public sealed class ConnectionSectionViewModel : Screen
  {
    private const string DestinationParam = "destination";
    private readonly ObservableCollection<object> _availableDestinations = new ObservableCollection<object>();
    private readonly IConnectionFactory _connectionFactory;
    private readonly IStationaryElement _stationaryElement;

    public ConnectionSectionViewModel(IStationaryElement stationaryElement,
                                      IConnectionFactory connectionFactory)
    {
      _connectionFactory = connectionFactory;
      _stationaryElement = stationaryElement;

      UpdateAvailableDestinations();


      DisplayName = "connections";

      // create a vm for each PT related distribution
      var connectionEditorViewModels = _stationaryElement.Connections.Select(con => new ConnectionEditorViewModel(con,
                                                                                                                  GetAvailableProductTypes(con.ProductTypes)));
      Connections = new ObservableCollection<ConnectionEditorViewModel>(connectionEditorViewModels);

      AddConnectionCommand = new ChainedParameterCommandViewModel(new[]
                                                                  {
                                                                    new ParameterSet(DestinationParam,
                                                                                     "Select a destination",
                                                                                     _availableDestinations)
                                                                  },
                                                                  AddConnection)
                             {
                               DisplayText = "Add"
                             };
      RemoveCommand = new RelayCommand(Remove);
    }

    public ObservableCollection<ConnectionEditorViewModel> Connections { get; private set; }
    public ChainedParameterCommandViewModel AddConnectionCommand { get; private set; }
    public ICommand RemoveCommand { get; private set; }

    private void AddConnection(IDictionary<string, object> parameters)
    {
      var destination = (IStationaryElement) parameters[DestinationParam];

      var connection = _connectionFactory.Create();
      connection.Destination = destination;
      _stationaryElement.Add(connection);

      var vm = new ConnectionEditorViewModel(connection,
                                             GetAvailableProductTypes(connection.ProductTypes));
      Connections.Add(vm);
      UpdateAvailableDestinations();
    }

    private void Remove(object item)
    {
      Connections.Remove((ConnectionEditorViewModel) item);
      var model = (IConnection) ((ConnectionEditorViewModel) item).Model;
      _stationaryElement.Remove(model);
      UpdateAvailableDestinations();
    }

    private void UpdateAvailableDestinations()
    {
      _availableDestinations.Clear();
      foreach (var destination in _stationaryElement.Model.Entities.OfType<IStationaryElement>()
                                                    .Where(se => se.CanConnectToSource(_stationaryElement) && _stationaryElement.CanConnectToDestination(se))
                                                    .OrderBy(se => se.Name))
      {
        _availableDestinations.Add(destination);
      }
    }

    private IEnumerable<IProductType> GetAvailableProductTypes(IEnumerable<IProductType> productTypesAlreadyInUse)
    {
      return _stationaryElement.Model.Entities.OfType<IProductType>()
                               .Except(productTypesAlreadyInUse);
    }
  }
}