#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using Caliburn.Micro;
using Emporer.WPF;
using Emporer.WPF.ViewModels;
using Milan.JsonStore;
using Milan.Simulation.UI.Factories;

namespace Milan.Simulation.UI.ViewModels
{
  [Export]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  public class ModelNavigatorViewModel : PropertyChangedBase
  {
    private readonly IModelNavigationViewModelFactory _modelViewModelFactory;
    private IEnumerable<object> _project;
    private object _selectedItem;
    private IViewModel _selectedViewModelItem;
    private ISelection _selection;

    [ImportingConstructor]
    public ModelNavigatorViewModel([Import] IDomainMessageBus messageBus, [Import] IModelNavigationViewModelFactory modelViewModelFactory)
    {
      _modelViewModelFactory = modelViewModelFactory;

      Models =
        new SortedObservableCollection<ModelNavigationViewModel>(
          Comparer<INavigatorNode>.Create((x, y) => string.Compare(x.DisplayName, y.DisplayName, false, CultureInfo.InvariantCulture)),
          "DisplayName");

      messageBus.EntityAdded.OfType<IModel>()
                .Subscribe(Add);

      messageBus.EntityRemoved.OfType<IModel>()
                .Subscribe(Remove);
    }

    public SortedObservableCollection<ModelNavigationViewModel> Models { get; private set; }

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

    private object SelectedItem
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
          //todo: war null und hat exception geworfen, hab mirs jetzt nicht angeschaut. ist also nen scneller hack
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
        _selection.Subscribe<object>(this, Select);
      }
    }

    private void Add(IModel model)
    {
      Models.Add(_modelViewModelFactory.CreateNavigationViewModel(model));
    }


    private void Remove(IModel model)
    {
      Models.Remove(Models.Single(x => x.Model == model));
    }

    private void UpdateToProject()
    {
      Models.Clear();
      SelectedItem = null;
      Project.OfType<IModel>()
             .Select(m => _modelViewModelFactory.CreateNavigationViewModel(m))
             .ForEach(vm => Models.Add(vm));
    }

    private void Select(object item)
    {
      if (item is IEnumerable)
      {
        // we can select only single items currently
        item = ((IEnumerable) item).Cast<object>()
                                   .FirstOrDefault();
      }

      ClearSelection(Models);
      Select(item, Models);
      _selectedItem = item;
    }

    private bool Select(object item, IEnumerable<INavigatorNode> nodes)
    {
      foreach (var node in nodes)
      {
        if (node.Model == item)
        {
          node.IsSelected = true;
          return true;
        }
        if (Select(item, node.Items))
        {
          node.IsExpanded = true;
          return true;
        }
      }
      return false;
    }

    private void ClearSelection(IEnumerable<INavigatorNode> nodes)
    {
      foreach (var node in nodes)
      {
        node.IsSelected = false;
        ClearSelection(node.Items);
      }
    }
  }
}