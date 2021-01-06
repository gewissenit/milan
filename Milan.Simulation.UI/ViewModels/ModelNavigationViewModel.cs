#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Caliburn.Micro;
using Emporer.WPF;
using Milan.Simulation.UI.Factories;

namespace Milan.Simulation.UI.ViewModels
{
  public class ModelNavigationViewModel : Screen, INavigatorNode
  {
    private readonly SortedObservableCollection<INavigatorNode> _entities;
    private readonly Func<IEntity, bool> _entityFilter;
    private readonly IEntityNavigationViewModelFactory _entityViewModelFactory;
    private readonly IModel _model;
    private bool _isExpanded;
    private bool _isSelected;


    public ModelNavigationViewModel(IModel model,
                                    IEntityNavigationViewModelFactory entityViewModelFactory,
                                    Func<IEntity, bool> entityFilter = null)

    {
      _model = model;
      _entityViewModelFactory = entityViewModelFactory;
      _entityFilter = entityFilter ?? (e => true);

      _entities =
        new SortedObservableCollection<INavigatorNode>(
          Comparer<INavigatorNode>.Create((x, y) => string.Compare(x.DisplayName, y.DisplayName, false, CultureInfo.InvariantCulture)),
          "DisplayName");

      _model.PropertyChanged += UpdateRelatedLocalProperty;

      _model.Entities.Where(_entityFilter)
             .ForEach(Add);

      _model.ObservableEntities.CollectionChanged += (s, e) =>
                                                      {
                                                        if (e.Action == NotifyCollectionChangedAction.Add)
                                                        {
                                                          e.NewItems.OfType<IEntity>()
                                                           .Where(_entityFilter)
                                                           .ForEach(Add);
                                                        }
                                                        if (e.Action == NotifyCollectionChangedAction.Remove)
                                                        {
                                                          e.OldItems.OfType<IEntity>()
                                                           .Where(_entityFilter)
                                                           .ForEach(Remove);
                                                        }
                                                      };
    }

    public bool IsSelected
    {
      get { return _isSelected; }
      set
      {
        if (_isSelected == value)
        {
          return;
        }
        _isSelected = value;
        NotifyOfPropertyChange(() => IsSelected);
      }
    }

    public bool IsExpanded
    {
      get { return _isExpanded; }
      set
      {
        if (_isExpanded == value)
        {
          return;
        }
        _isExpanded = value;
        NotifyOfPropertyChange(() => IsExpanded);
      }
    }

    public ObservableCollection<INavigatorNode> Items
    {
      get { return _entities; }
    }

    public object Model
    {
      get { return _model; }
    }

    private void Add(IEntity entity)
    {
      if (Items.Any(x => x.Model == entity))
      {
        return;
        // can happen, e.g. a model is duplicated: model is added with all it entities, after that all entities get 'added' -> would lead to duplicated items
      }
      Items.Add(CreateEntityNavigationViewModel(entity));
    }

    private void Remove(IEntity entity)
    {
      Items.Remove(Items.Single(e => e.Model == entity));
    }

    private INavigatorNode CreateEntityNavigationViewModel(IEntity entity)
    {
      return _entityViewModelFactory.CreateNavigationViewModel(entity);
    }

    private void UpdateRelatedLocalProperty(object sender, PropertyChangedEventArgs e)
    {
      switch (e.PropertyName)
      {
        case "Name":
          DisplayName = _model.Name;
          break;
      }
    }
  }
}