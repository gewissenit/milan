#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;
using Caliburn.Micro;
using Emporer.WPF.Commands;
using Emporer.WPF.Factories;
using Milan.Simulation.Factories;
using Milan.Simulation.Observers;

namespace Milan.Simulation.UI.ViewModels
{
  [Export]
  public class ExperimentControlViewModel : Screen
  {
    private readonly IEnumerable<IEditViewModelFactory> _editViewModelFactories;
    private readonly ISimulationService _simulationService;
    private readonly IEnumerable<ITerminationCriteriaFactory> _terminationCriteriaFactories;
    private IHaveDisplayName _editItem;

    private int _numberOfRuns = 1;
    private IModel _selectedModel;
    private TimeSpan _settlingTime = new TimeSpan(0);

    [ImportingConstructor]
    public ExperimentControlViewModel([Import] ISimulationService simulationService, [ImportMany] IEnumerable<ITerminationCriteriaFactory> terminationCriteriaFactories, [ImportMany] IEnumerable<IStatisticalObserverFactory> statisticalObserverFactories, [ImportMany] IEnumerable<IEditViewModelFactory> editViewModelFactories)
    {
      _editViewModelFactories = editViewModelFactories;
      DisplayName = "Experiment Control";

      _simulationService = simulationService;
      _terminationCriteriaFactories = terminationCriteriaFactories;
      StatisticalObserverFactories = statisticalObserverFactories.Select(o => new StatisticalObserverFactoryViewModel(o, true))
                                                                 .ToArray();

      ExecuteBatchCommand = new RelayCommand(ExecuteBatch, CanExecuteBatch);
      UpdateAllCommandStates();
    }

    public ObservableCollection<TerminationCriterionViewModel> TerminationCriteria { get; private set; }

    public IEnumerable<StatisticalObserverFactoryViewModel> StatisticalObserverFactories { get; }

    // commands
    public RelayCommand ExecuteBatchCommand { get; private set; }

    public bool AllStatisticalObserverSelected
    {
      get { return StatisticalObserverFactories.All(sof => sof.IsSelected); }
    }

    public bool AllTerminationCriteriaSelected
    {
      get { return TerminationCriteria != null && TerminationCriteria.All(sof => sof.IsSelected); }
    }

    public int NumberOfRuns
    {
      get { return _numberOfRuns; }
      set
      {
        if (_numberOfRuns == value)
        {
          return;
        }
        _numberOfRuns = value;
        NotifyOfPropertyChange(() => NumberOfRuns);
      }
    }

    public DateTime StartDay
    {
      get
      {
        return _selectedModel == null
                 ? DateTime.Now
                 : _selectedModel.StartDate;
      }
      set
      {
        if (_selectedModel == null)
        {
          return;
        }
        var newStartDate = new DateTime(value.Year, value.Month, value.Day, _selectedModel.StartDate.Hour, _selectedModel.StartDate.Minute, _selectedModel.StartDate.Second, _selectedModel.StartDate.Millisecond);
        _selectedModel.StartDate = newStartDate;
        NotifyOfPropertyChange(() => StartDay);
      }
    }

    public DateTime StartTime
    {
      get
      {
        return _selectedModel == null
                 ? DateTime.Now
                 : _selectedModel.StartDate;
      }
      set
      {
        if (_selectedModel == null)
        {
          return;
        }
        var newStartDate = new DateTime(_selectedModel.StartDate.Year, _selectedModel.StartDate.Month, _selectedModel.StartDate.Day, value.Hour, value.Minute, value.Second, value.Millisecond);
        _selectedModel.StartDate = newStartDate;
        NotifyOfPropertyChange(() => StartTime);
      }
    }

    public TimeSpan SettlingTime
    {
      get { return _settlingTime; }
      set
      {
        if (_settlingTime == value)
        {
          return;
        }
        _settlingTime = value;
        NotifyOfPropertyChange(() => SettlingTime);
      }
    }

    public IModel SelectedModel
    {
      get { return _selectedModel; }
      set
      {
        if (_selectedModel == value)
        {
          return;
        }
        _selectedModel = value;
        UpdateToSelectedModelChange();
        NotifyOfPropertyChange(() => SelectedModel);
      }
    }

    public IHaveDisplayName EditItem
    {
      get { return _editItem; }
      private set
      {
        _editItem = value;
        NotifyOfPropertyChange(() => EditItem);
      }
    }


    public event EventHandler<AggregateException> ErrorOccurredDuringBatchRun
    {
      add { _simulationService.BatchError += value; }
      remove { _simulationService.BatchError -= value; }
    }

    // execute batch
    private bool CanExecuteBatch(object notUsed)
    {
      return SelectedModel != null;
    }

    private void ExecuteBatch(object notUsed)
    {
      var batch = _simulationService.CreateBatch(SelectedModel, NumberOfRuns, StatisticalObserverFactories.Where(sof => sof.IsSelected)
                                                                                                          .Select(sof => sof.Model)
                                                                                                          .ToArray(), SettlingTime);


      _simulationService.Start(batch);
    }


    public void AddTerminationCriterion(object dataContext)
    {
      var tcvm = (TerminationCriterionViewModel) dataContext;
      _selectedModel.Add(tcvm.Model);
    }

    public void RemoveTerminationCriterion(object dataContext)
    {
      var tcvm = (TerminationCriterionViewModel) dataContext;
      _selectedModel.Remove(tcvm.Model);
    }

    public void UpdateToTerminationCriteriaSelection(SelectionChangedEventArgs eventArgs)
    {
      if (eventArgs.AddedItems.Count == 0)
      {
        return;
      }
      var tcvm = (TerminationCriterionViewModel) eventArgs.AddedItems[0];
      if (tcvm.IsSelected)
      {
        EditTerminationCriteria(tcvm);
      }
      else
      {
        ClearTerminationEditor();
      }
    }

    private void ClearTerminationEditor()
    {
      EditItem = null;
    }

    private void EditTerminationCriteria(TerminationCriterionViewModel tcvm)
    {
      EditItem = _editViewModelFactories.Single(w => w.CanHandle(tcvm.Model))
                                        .CreateEditViewModel(tcvm.Model);
    }

    public void SelectAllTerminationCriteria()
    {
      foreach (var tc in TerminationCriteria)
      {
        tc.IsSelected = true;
      }
    }

    public void DeselectAllTerminationCriteria()
    {
      foreach (var tc in TerminationCriteria)
      {
        tc.IsSelected = false;
      }
    }

    public void SelectAllStatisticalObserver()
    {
      foreach (var factory in StatisticalObserverFactories)
      {
        factory.IsSelected = true;
      }
    }

    public void DeselectAllStatisticalObserver()
    {
      foreach (var factory in StatisticalObserverFactories)
      {
        factory.IsSelected = false;
      }
    }

    private void UpdateToSelectedModelChange()
    {
      CreateTerminationCriteria();
      UpdateAllCommandStates();
      ClearTerminationEditor();
      NotifyOfPropertyChange(() => StartDay);
      NotifyOfPropertyChange(() => StartTime);
    }

    private void CreateTerminationCriteria()
    {
      //todo: optimize
      var modelTerminationCriteria = _selectedModel.Observers.OfType<ITerminationCriteria>()
                                                   .ToArray();
      var terminationCriteria = _terminationCriteriaFactories.Select(tcf => tcf.Create());
      terminationCriteria = modelTerminationCriteria.Aggregate(terminationCriteria, (current, modelTerminationCriterion) => current.Where(tc => tc.GetType() != modelTerminationCriterion.GetType()));

      TerminationCriteria = new ObservableCollection<TerminationCriterionViewModel>((terminationCriteria.Select(tc => new TerminationCriterionViewModel(tc, false))
                                                                                                        .Concat(modelTerminationCriteria.Select(tc => new TerminationCriterionViewModel(tc, true)))));

      NotifyOfPropertyChange(() => TerminationCriteria);
    }

    private void UpdateAllCommandStates()
    {
      ExecuteBatchCommand.UpdateCanExecute();
    }
  }
}