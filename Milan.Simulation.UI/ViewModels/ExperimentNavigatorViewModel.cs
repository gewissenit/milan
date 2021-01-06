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
using Emporer.WPF.ViewModels;
using Milan.JsonStore;
using Milan.Simulation.UI.Factories;

namespace Milan.Simulation.UI.ViewModels
{
  [Export]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  public class ExperimentNavigatorViewModel : PropertyChangedBase
  {
    private readonly IExperimentNavigatorModelViewModelFactory _modelViewModelFactory;
    private IEnumerable<object> _project;
    private object _selectedItem;
    private IViewModel _selectedViewModelItem;

    [ImportingConstructor]
    public ExperimentNavigatorViewModel([Import] IDomainMessageBus messageBus, IExperimentNavigatorModelViewModelFactory modelViewModelFactory)
    {
      _modelViewModelFactory = modelViewModelFactory;
      Models = new ObservableCollection<ExperimentNavigationViewModel>();

      messageBus.EntityAdded.OfType<IModel>()
                  .Subscribe(Add);

      messageBus.EntityRemoved.OfType<IModel>()
                  .Subscribe(Remove);
    }

    public ObservableCollection<ExperimentNavigationViewModel> Models { get; private set; }

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
      Project.OfType<IModel>()
             .Select(m => _modelViewModelFactory.CreateNavigationViewModel(m))
             .ForEach(vm => Models.Add(vm));
    }
  }
}