using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using Emporer.WPF.Commands;
using Emporer.WPF.ViewModels;

namespace Milan.Simulation.Resources.UI.ViewModels
{
  public sealed class InfluencesSectionViewModel : Screen
  {
    private readonly ObservableCollection<object> _availableInfluences;
    private readonly IResourceType _model;

    public InfluencesSectionViewModel(IResourceType model)
    {
      _model = model;

      DisplayName = "influences";

      _availableInfluences = new ObservableCollection<object>(_model.Model.Entities.OfType<IInfluence>()
                                                                    .Except(_model.Influences.Select(i => i.Influence)));

      Influences = new ObservableCollection<ResourceTypeInfluenceEditorViewModel>(_model.Influences.Select(i => new ResourceTypeInfluenceEditorViewModel(i))
                                                                                        .ToList());
      AddInfluenceCommand = CreateAddInfluenceCommand("Add",
                                                      _availableInfluences,
                                                      AddInfluence);
      RemoveCommand = new RelayCommand(RemoveInfluence);
    }

    public ObservableCollection<ResourceTypeInfluenceEditorViewModel> Influences { get; private set; }
    public ChainedParameterCommandViewModel AddInfluenceCommand { get; private set; }
    public ICommand RemoveCommand { get; private set; }

    private void AddInfluence(IDictionary<string, object> parameters)
    {
      AddInfluence(parameters,
                   vm =>
                   {
                     Influences.Add(vm);
                     _availableInfluences.Remove(vm.Model.Influence);
                   },
                   _model.Add);
    }

    private void AddInfluence(IDictionary<string, object> parameters,
                              Action<ResourceTypeInfluenceEditorViewModel> updateViewModel,
                              Action<IResourceTypeInfluence> addToModel)
    {
      var influence = (IInfluence) parameters["influence"];
      var resourceTypeInfluence = new ResourceTypeInfluence
                                  {
                                    Influence = influence
                                  };
      addToModel(resourceTypeInfluence);

      var vm = new ResourceTypeInfluenceEditorViewModel(resourceTypeInfluence);

      updateViewModel(vm);
    }

    private ChainedParameterCommandViewModel CreateAddInfluenceCommand(string displayText,
                                                                       ObservableCollection<object> availableInfluences,
                                                                       Action<IDictionary<string, object>> addAction)
    {
      var parameterSet = new ParameterSet("influence",
                                          "Select an influence",
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

    private void RemoveInfluence(object item)
    {
      var influence = (ResourceTypeInfluenceEditorViewModel) item;

      _model.Remove(influence.Model);
      Influences.Remove(influence);
      _availableInfluences.Add(influence.Model.Influence);
      NotifyOfPropertyChange(() => AddInfluenceCommand);
    }
  }
}