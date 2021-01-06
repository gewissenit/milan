#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using Caliburn.Micro;
using Emporer.Material;
using Emporer.WPF;
using Emporer.WPF.Commands;
using Milan.JsonStore;
using Milan.Simulation.MaterialAccounting.Factories;
using Milan.Simulation.MaterialAccounting.UI.Factories;
using Milan.Simulation.Observers;

namespace Milan.Simulation.MaterialAccounting.UI.ViewModels
{
  [Export]
  public class MaterialFlowsViewModel : Screen
  {
    private readonly ObservableCollection<string> _categories = new ObservableCollection<string>();
    private readonly RelayCommand _deleteSelectedObserverCommand;
    private readonly ObservableCollection<IMaterialObserverViewModel> _displayedObservers = new ObservableCollection<IMaterialObserverViewModel>();
    private readonly RelayCommand _duplicateSelectedObserverCommand;
    private readonly IEnumerable<IMaterialObserverFactory> _materialObserverFactories;
    private readonly IMaterialObserverViewModelFactory _materialObserverViewModelFactory;
    private readonly IJsonStore _store;

    private IEntity _activeEntity;
    private IModel _activeModel;
    private AddMaterialObserverChainedCommand _addObserverChainedCommand;
    private object _editItem;
    private IMaterialObserverViewModel _selectedItem;
    private string _selectionDescription;
    private ISelection _selection;

    [ImportingConstructor]
    public MaterialFlowsViewModel([ImportMany] IEnumerable<IMaterialObserverFactory> materialObserverFactories,
                                  [Import] IMaterialObserverViewModelFactory materialObserverViewModelFactory,
                                  [Import] IJsonStore store)
    {
      _materialObserverFactories = materialObserverFactories;
      _materialObserverViewModelFactory = materialObserverViewModelFactory;

      _store = store;
      
      _store.ProjectChanged.Subscribe(_=> UpdateToProject());

      DisplayName = "Material Flows";

      Items = new CollectionViewSource
              {
                Source = _displayedObservers
              };

      UpdateToCurrentSelection();

      _duplicateSelectedObserverCommand = new RelayCommand(DuplicateSelectedObserver, CanDuplicateSelectedObserver);
      _deleteSelectedObserverCommand = new RelayCommand(DeleteSeletedObserver, CanDeleteSelectedObserver);
      if (_activeModel == null)
      {
        return;
      }
      UpdateAddObserverCommandViewModel();
    }

    public object EditItem
    {
      get { return _editItem; }
      set
      {
        if (_editItem == value)
        {
          return;
        }
        _editItem = value;
        UpdateToCurrentSelection();
        NotifyOfPropertyChange(() => EditItem);
      }
    }

    public AddMaterialObserverChainedCommand AddObserverChainedCommand
    {
      get { return _addObserverChainedCommand; }
      set
      {
        if (_addObserverChainedCommand == value)
        {
          return;
        }
        _addObserverChainedCommand = value;
        NotifyOfPropertyChange(() => AddObserverChainedCommand);
      }
    }

    public ICommand DeleteSelectedObserverCommand
    {
      get { return _deleteSelectedObserverCommand; }
    }

    public ICommand DuplicateSelectedObserverCommand
    {
      get { return _duplicateSelectedObserverCommand; }
    }

    public CollectionViewSource Items { get; set; }

    public IMaterialObserverViewModel SelectedItem
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
        _deleteSelectedObserverCommand.UpdateCanExecute();
        _duplicateSelectedObserverCommand.UpdateCanExecute();
      }
    }

    public string SelectionDescription
    {
      get { return _selectionDescription; }
      set
      {
        if (_selectionDescription == value)
        {
          return;
        }
        _selectionDescription = value;
        NotifyOfPropertyChange(() => SelectionDescription);
      }
    }

    public ISelection Selection
    {
      get { return _selection; }
      set
      {
        _selection = value;
        _selection.Subscribe<object>(this, Select);
      }
    }
    private void Select(object item)
    {
      EditItem = item;
    }

    private void UpdateToProject()
    {
      _activeModel = null;
      EditItem = null;
    }

    private void AddObserver(IMaterialObserver observer, IMaterial material, IEntity entity//, BalanceDirection balanceSide
      )
    {
      observer.Material = material;
      observer.Amount = 1d;
      observer.LossRatio = 0d;
      //observer.BalanceSide = balanceSide;
      observer.Entity = entity;
      _activeModel.Add(observer);

      UpdateToCurrentSelection();
      UpdateAddObserverCommandViewModel();
      SelectObserverIfVisible(observer);
    }

    private bool CanDeleteSelectedObserver(object obj)
    {
      return SelectedItem != null;
    }

    private bool CanDuplicateSelectedObserver(object obj)
    {
      return SelectedItem != null;
    }

    private void ClearSelection()
    {
      if (_activeModel != null)
      {
        UpdateObserversOfSelectedModel();
      }
      else
      {
        _activeEntity = null;
        _displayedObservers.Clear();
        SelectionDescription = "Nothing selected.";
      }
    }

    private void DeleteSeletedObserver(object obj)
    {
      var observerToDelete = (IMaterialObserver) SelectedItem.Model;
      _activeModel.Remove(observerToDelete);
      UpdateToCurrentSelection();
    }

    private void DuplicateSelectedObserver(object obj)
    {
      var observerToDuplicate = (IMaterialObserver) SelectedItem.Model;

      var clone = _materialObserverFactories.Single(mof => mof.CanHandle(observerToDuplicate))
                                            .Duplicate(observerToDuplicate);
      _activeModel.Add(clone);
      UpdateToCurrentSelection();
      SelectObserverIfVisible(clone);
    }

    private void SelectObserverIfVisible(IMaterialObserver observer)
    {
      var visibleObserverViewModel = _displayedObservers.SingleOrDefault(x => x.Model == observer);

      if (visibleObserverViewModel == null)
      {
        return;
      }
      SelectedItem = visibleObserverViewModel;
    }

    private void UpdateAddObserverCommandViewModel()
    {
      AddObserverChainedCommand = new AddMaterialObserverChainedCommand(_store, _materialObserverFactories, _activeModel, _activeEntity, AddObserver);
    }


    private void UpdateCategories()
    {
      var categoriesInUse = _activeModel.Observers.OfType<IMaterialObserver>()
                                        .Select(x => x.Category)
                                        .ToArray();

      foreach (var existingCategory in _categories.ToArray()
                                                  .Where(existingCategory => !categoriesInUse.Contains(existingCategory)))
      {
        _categories.Remove(existingCategory);
      }

      foreach (var category in categoriesInUse.Where(category => !_categories.Contains(category)))
      {
        _categories.Add(category);
      }
    }

    /// <summary>
    ///   Find and viewmodel the observers of the selected entity.
    /// </summary>
    /// <param name="entity">The selected entity.</param>
    private void UpdateObserversOfSelectedEntity(IEntity entity)
    {
      if (_activeModel == null)
      {
        throw new InvalidOperationException("Can not update without an active model.");
      }
      if (entity == null)
      {
        throw new ArgumentNullException("entity");
      }

      _activeEntity = entity;

      SelectionDescription = string.Format("Entity '{0}' selected.", _activeEntity.Name);

      _displayedObservers.Clear();

      var materialObservers = _activeModel.Observers.OfType<IEntityObserver>()
                                          .Where(mt => mt.Entity == _activeEntity)
                                          .OfType<IMaterialObserver>()
                                          .OrderBy(x => x.Entity.Name)
                                          .ThenBy(x => x.Name)
                                          .ThenBy(x => x.Material.Name);

      foreach (var materialObserver in materialObservers)
      {
        _displayedObservers.Add(_materialObserverViewModelFactory.Create(materialObserver, entity.Model.Entities.OfType<IProductType>(), _categories));
      }

      UpdateAddObserverCommandViewModel();
    }


    /// <summary>
    ///   Find and viewmodel the observers of all entities in the model.
    /// </summary>
    private void UpdateObserversOfSelectedModel()
    {
      if (_activeModel == null)
      {
        throw new InvalidOperationException();
      }

      SelectionDescription = string.Format("Model '{0}' selected.", _activeModel.Name);
      _activeEntity = null;
      _displayedObservers.Clear();

      var materialObservers = _activeModel.Observers.OfType<IMaterialObserver>()
                                          .OrderBy(x => x.Entity.Name)
                                          .ThenBy(x => x.Name)
                                          .ThenBy(x => x.Material.Name);
      foreach (var materialObserver in materialObservers)
      {
        _displayedObservers.Add(_materialObserverViewModelFactory.Create(materialObserver, _activeModel.Entities.OfType<IProductType>(), _categories));
      }

      UpdateAddObserverCommandViewModel();
    }

    /// <summary>
    ///   On selection change, decide wether a model, an entity or nothing was selected.
    /// </summary>
    private void UpdateToCurrentSelection()
    {
      if (_editItem is IModel)
      {
        // model was directly selected, change active model
        _activeModel = (IModel) _editItem;
        UpdateCategories();
        UpdateObserversOfSelectedModel();
      }
      else if (_editItem is IEntity)
      {
        // another entity was selected
        var entity = (IEntity) _editItem;
        _activeModel = entity.Model;
        UpdateCategories();
        UpdateObserversOfSelectedEntity(entity);
      }
      else
      {
        ClearSelection();
      }
    }
  }
}