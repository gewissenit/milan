#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Linq;
using Caliburn.Micro;
using EcoFactory.Components;
using Emporer.WPF;
using Emporer.WPF.ViewModels;
using Milan.JsonStore;
using Milan.Simulation;
using Milan.Simulation.UI.Factories;
using Milan.Simulation.UI.ViewModels;

namespace Milan.UI.ViewModels
{
  [Export]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  public class FlowsNavigatorViewModel : PropertyChangedBase
  {
    private readonly IModelNavigationViewModelFactory _modelViewModelFactory;
    private IEnumerable<object> _project;
    private object _selectedItem;
    private IViewModel _selectedViewModelItem;
    private ISelection _selection;

    [ImportingConstructor]
    public FlowsNavigatorViewModel([Import] IDomainMessageBus messageBus, [Import] IModelNavigationViewModelFactory modelViewModelFactory)
    {
      _modelViewModelFactory = modelViewModelFactory;
      Models = new ObservableCollection<ModelNavigationViewModel>();

      messageBus.EntityAdded.OfType<IModel>()
                  .Subscribe(Add);

      messageBus.EntityRemoved.OfType<IModel>()
                  .Subscribe(Remove);
    }

    public ObservableCollection<ModelNavigationViewModel> Models { get; private set; }

    public IEnumerable<object> Project
    {
      get { return _project; }
      set
      {
        if (value == null)
        {
          throw new ArgumentNullException();
        }

        if (_project == value)
        {
          return;
        }

        _project = value;
        UpdateToProject();

        NotifyOfPropertyChange(() => Project);
      }
    }

    public object SelectedItem
    {
      get { return _selectedItem; }
      set
      {
        if (_selectedItem == value)
        {
          return;
        }
        _selectedItem = value;
        NotifyOfPropertyChange(() => SelectedItem);
        _selection.Select(_selectedItem, this);
      }
    }


    public IViewModel SelectedViewModelItem
    {
      get { return _selectedViewModelItem; }
      set
      {
        if (_selectedViewModelItem == value)
        {
          return;
        }
        _selectedViewModelItem = value;
        NotifyOfPropertyChange(() => SelectedViewModelItem);

        if (_selectedViewModelItem != null)
        {
          SelectedItem = _selectedViewModelItem.Model;
        }
      }
    }

    public ISelection Selection
    {
      get { return _selection; }
      set
      {
        _selection = value;
      }
    }

    private void Add(IModel model)
    {
      var modelVm = _modelViewModelFactory.CreateNavigationViewModel(model, IsObservableEntity);
      Models.Add(modelVm);
    }


    private void Remove(IModel model)
    {
      var modelToRemove = Models.Single(x => x.Model == model);
      Models.Remove(modelToRemove);
    }

    private void UpdateToProject()
    {
      Models.Clear();
      Project.OfType<IModel>()
             .ForEach(Add);
    }

    private static bool IsObservableEntity(IEntity entity)
    {
      return entity is IWorkstation || entity is IParallelWorkstation || entity is IInhomogeneousParallelWorkstation ||
             entity is IInhomogeneousWorkstation || entity is IAssemblyStation || entity is IProbabilityAssemblyStation || entity is IExitPoint ||
             entity is IEntryPoint;
    }
  }
}