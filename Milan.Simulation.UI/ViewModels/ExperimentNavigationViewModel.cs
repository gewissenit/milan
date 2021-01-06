#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Caliburn.Micro;
using Emporer.WPF.ViewModels;

namespace Milan.Simulation.UI.ViewModels
{
  public class ExperimentNavigationViewModel : Screen, IViewModel
  {
    private readonly IModel _model;

    public ExperimentNavigationViewModel(IModel model, ISimulationService simulationService)
    {
      Batches = new ObservableCollection<BatchViewModel>();

      _model = model;
      _model.PropertyChanged += UpdateRelatedLocalProperty;

      simulationService.BatchCreated += AddBatchToChildren;
      simulationService.BatchDeleted += RemoveBatchFromchildren;
    }

    public ObservableCollection<BatchViewModel> Batches { get; private set; }

    public object Model
    {
      get { return _model; }
    }

    private void RemoveBatchFromchildren(object sender, BatchEventArgs e)
    {
      if (e.Batch.Model != _model)
      {
        return;
      }
      RemoveBatchFromchildren(e.Batch);
    }

    private void RemoveBatchFromchildren(IBatch batch)
    {
      Batches.Remove(Batches.Single(x => x.Model == batch));
    }

    private void AddBatchToChildren(object sender, BatchEventArgs e)
    {
      if (e.Batch.Model != _model)
      {
        return;
      }
      AddBatchToChildren(e.Batch);
    }

    private void AddBatchToChildren(IBatch batch)
    {
      var vm = new BatchViewModel(batch);
      Batches.Add(vm);
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