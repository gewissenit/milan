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
using Emporer.Unit;
using Emporer.Unit.Factories;
using Emporer.WPF;
using Emporer.WPF.Commands;
using Milan.JsonStore;
using Milan.Simulation.CostAccounting.Factories;
using Milan.Simulation.CostAccounting.UI.Factories;
using Milan.Simulation.Observers;

namespace Milan.Simulation.CostAccounting.UI.ViewModels
{
  [Export]
  public class CostFlowsViewModel : Screen
  {
    private readonly ObservableCollection<string> _categories = new ObservableCollection<string>();
    private readonly IEnumerable<ICostObserverFactory> _costObserverFactories;
    private readonly ICostObserverViewModelFactory _costObserverViewModelFactory;
    private readonly RelayCommand _deleteSelectedObserverCommand;
    private readonly ObservableCollection<ICostObserverViewModel> _displayedObservers = new ObservableCollection<ICostObserverViewModel>();
    private readonly RelayCommand _duplicateSelectedObserverCommand;
    private readonly IStandardUnitFactory _standardUnitFactory;
    private readonly IJsonStore _store;
    private IEntity _activeEntity;
    private IModel _activeModel;
    private AddCostObserverChainedCommand _addObserverChainedCommand;
    private IUnit[] _currencies;
    private object _editItem;
    private ICostObserverViewModel _selectedItem;
    private string _selectionDescription;
    private ISelection _selection;

    [ImportingConstructor]
    public CostFlowsViewModel([ImportMany] IEnumerable<ICostObserverFactory> costObserverFactories,
                              [Import] ICostObserverViewModelFactory materialObserverViewModelFactory,
                              [Import] IStandardUnitFactory standardUnitFactory,
                              [Import] IJsonStore store)
    {
      DisplayName = "Cost Flows";
      _costObserverFactories = costObserverFactories;
      _costObserverViewModelFactory = materialObserverViewModelFactory;
      _standardUnitFactory = standardUnitFactory;
      _store = store;
      _currencies = _standardUnitFactory.StandardUnits.Where(u => u.Dimension == "Currency")
                                        .ToArray();
      _store.ProjectChanged.Subscribe(_ => UpdateToProject());

      Items = new CollectionViewSource
              {
                Source = _displayedObservers
              };

      UpdateToCurrentSelecion();

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
        UpdateToCurrentSelecion();
        NotifyOfPropertyChange(() => EditItem);
      }
    }

    public AddCostObserverChainedCommand AddObserverChainedCommand
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

    public ICostObserverViewModel SelectedItem
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
      EditItem =  item;
    }

    private void UpdateToProject()
    {
      _activeModel = null;
      EditItem = null;
      _currencies = _standardUnitFactory.StandardUnits.Where(u => u.Dimension == "Currency")
                                        .ToArray();
    }

    private void AddObserver(ICostObserver observer, IUnit currency, IEntity entity)
    {
      observer.Currency = currency;
      observer.Amount = 1d;
      observer.Entity = entity;

      _activeModel.Add(observer);

      UpdateToCurrentSelecion();
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
        UpdateToAllObserversOfSelectedModel();
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
      var observerToDelete = (ICostObserver) SelectedItem.Model;
      _activeModel.Remove(observerToDelete);
      UpdateToCurrentSelecion();
    }

    private void DuplicateSelectedObserver(object obj)
    {
      var observerToDuplicate = (ICostObserver) SelectedItem.Model;

      var clone = _costObserverFactories.Single(cof => cof.CanHandle(observerToDuplicate))
                                        .Duplicate(observerToDuplicate);
      _activeModel.Add(clone);

      UpdateToCurrentSelecion();
      SelectObserverIfVisible(clone);
    }

    private void SelectObserverIfVisible(ICostObserver observer)
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
      AddObserverChainedCommand = new AddCostObserverChainedCommand(_costObserverFactories, _currencies, _activeModel, _activeEntity, AddObserver);
    }

    private void UpdateCategories()
    {
      var categoriesInUse = _activeModel.Observers.OfType<ICostObserver>()
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
                                          .OfType<ICostObserver>()
                                          .OrderBy(x => x.Entity.Name)
                                          .ThenBy(x => x.Name)
                                          .ThenBy(x => x.Currency.Symbol);

      foreach (var materialObserver in materialObservers)
      {
        _displayedObservers.Add(_costObserverViewModelFactory.Create(materialObserver,
                                                                     entity.Model.Entities.OfType<IProductType>(),
                                                                     _currencies,
                                                                     _categories));
      }

      UpdateAddObserverCommandViewModel();
    }


    /// <summary>
    ///   Find and viewmodel the observers of all entities in the model.
    /// </summary>
    private void UpdateToAllObserversOfSelectedModel()
    {
      if (_activeModel == null)
      {
        throw new InvalidOperationException();
      }

      SelectionDescription = string.Format("Model '{0}' selected.", _activeModel.Name);
      _activeEntity = null;
      _displayedObservers.Clear();

      var materialObservers = _activeModel.Observers.OfType<ICostObserver>()
                                          .OrderBy(x => x.Entity.Name)
                                          .ThenBy(x => x.Name)
                                          .ThenBy(x => x.Currency.Symbol);
      foreach (var materialObserver in materialObservers)
      {
        _displayedObservers.Add(_costObserverViewModelFactory.Create(materialObserver,
                                                                     _activeModel.Entities.OfType<IProductType>(),
                                                                     _currencies,
                                                                     _categories));
      }

      UpdateAddObserverCommandViewModel();
    }

    /// <summary>
    ///   On selection change, decide wether a model, an entity or nothing was selected.
    /// </summary>
    private void UpdateToCurrentSelecion()
    {
      if (_editItem is IModel)
      {
        _activeModel = (IModel) _editItem;
        UpdateCategories();
        UpdateToAllObserversOfSelectedModel();
      }
      else if (_editItem is IEntity)
      {
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