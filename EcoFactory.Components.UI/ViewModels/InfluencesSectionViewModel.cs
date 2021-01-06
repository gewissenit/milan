using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using Emporer.WPF.Commands;
using Emporer.WPF.ViewModels;
using Milan.Simulation.Resources;
using Milan.Simulation.Resources.UI.ViewModels;

namespace EcoFactory.Components.UI.ViewModels
{
  public sealed class InfluencesSectionViewModel : Screen
  {
    private readonly ObservableCollection<object> _availableInfluences;
    private readonly IWorkstationBase _model;

    public InfluencesSectionViewModel(IWorkstationBase model)
    {
      _model = model;

      DisplayName = "influences";

      _availableInfluences = new ObservableCollection<object>(_model.Model.Entities.OfType<IInfluence>()
                                                                    .Except(_model.Influences.Select(i => i.Influence)));

      Influences = new ObservableCollection<InfluenceRateEditorViewModel>(_model.Influences.Select(i => new InfluenceRateEditorViewModel(i))
                                                                                .ToList());
      AddInfluenceCommand = CreateAddInfluenceCommand("Add",
                                                      _availableInfluences,
                                                      AddInfluence);
      RemoveCommand = new RelayCommand(Remove);
    }

    public ICommand RemoveCommand { get; private set; }
    public ObservableCollection<InfluenceRateEditorViewModel> Influences { get; private set; }
    public ChainedParameterCommandViewModel AddInfluenceCommand { get; private set; }

    private ChainedParameterCommandViewModel CreateAddInfluenceCommand(string displayText,
                                                                       ObservableCollection<object> availableInfluences,
                                                                       Action<IDictionary<string, object>> addAction)
    {
      var parameterSet = new ParameterSet("influence",
                                          "Select a product type",
                                          availableInfluences);

      var chainedParameterCommand = new ChainedParameterCommandViewModel(new[]
                                                                         {
                                                                           parameterSet
                                                                         },
                                                                         addAction)
                                    {
                                      DisplayText = displayText
                                    };

      return chainedParameterCommand;
    }

    private void AddInfluence(IDictionary<string, object> parameters,
                              Action<InfluenceRateEditorViewModel> updateViewModel,
                              Action<IInfluenceRate> addToModel)
    {
      var influence = (IInfluence) parameters["influence"];
      var rate = new InfluenceRate
                 {
                   Influence = influence
                 };
      addToModel(rate);
      //todo: use factory
      var vm = new InfluenceRateEditorViewModel(rate);

      updateViewModel(vm);
    }

    private void AddInfluence(IDictionary<string, object> parameters)
    {
      AddInfluence(parameters,
                   vm =>
                   {
                     Influences.Add(vm);
                     _availableInfluences.Remove(vm.Model);
                   },
                   _model.AddInfluence);
    }

    private void RemoveInfluence(InfluenceRateEditorViewModel item)
    {
      _model.RemoveInfluence(item.Model);
      Influences.Remove(item);
      _availableInfluences.Add(item.Model);
      NotifyOfPropertyChange(() => AddInfluenceCommand);
    }

    private void Remove(object item)
    {
      RemoveInfluence((InfluenceRateEditorViewModel) item);
    }
  }
}