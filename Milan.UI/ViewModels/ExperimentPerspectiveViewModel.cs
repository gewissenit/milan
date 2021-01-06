#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Milan.JsonStore;
using Milan.Simulation;
using Milan.Simulation.UI.ViewModels;
using Ork.Framework;

namespace Milan.UI.ViewModels
{
  [Export(typeof (IWorkspace))]
  public class ExperimentPerspectiveViewModel : DocumentBase, IWorkspace
  {
    private readonly ISimulationService _simulationService;
    private readonly IJsonStore _store;

    [ImportingConstructor]
    public ExperimentPerspectiveViewModel([Import] ExperimentNavigatorViewModel experimentNavigatorViewModel,
                                          [Import] ExperimentControlViewModel experimentControlViewModel,
                                          [Import] ISimulationService simulationService,
                                          [Import] IJsonStore store)
    {
      _simulationService = simulationService;
      Index = 40;
      DisplayName = "Experiments";
      IsEnabled = true;
      _store = store;
      ExperimentNavigator = experimentNavigatorViewModel;
      ExperimentControl = experimentControlViewModel;

      _store.ProjectChanged.Subscribe(_=> OnProjectChanged());
      ExperimentNavigator.PropertyChanged += UpdateToSelectedItem;

      ExperimentControl.ErrorOccurredDuringBatchRun += ShowBatchRunErrorDialog;

      SetCurrentProject();
    }
    
    public ExperimentControlViewModel ExperimentControl { get; private set; }
    public ExperimentNavigatorViewModel ExperimentNavigator { get; private set; }

    public bool CanOpenBatchDataFolder
    {
      get { return ExperimentNavigator.SelectedItem is IBatch; }
    }

    public bool CanRemoveSelected
    {
      get { return ExperimentNavigator.SelectedItem is IBatch; }
    }

    public int Index { get; private set; }
    public bool IsEnabled { get; private set; }

    private void ShowBatchRunErrorDialog(object sender, AggregateException e)
    {
      var error = "Errors have occured during the batch execution!" + Environment.NewLine;
      error += "##Batch" + Environment.NewLine;
      error += sender + Environment.NewLine;
      error += "##Cause" + Environment.NewLine;
      error = e.InnerExceptions.Select(ie => ie.Message)
               .Aggregate(error, (current, message) => current + message + Environment.NewLine);
      Dialogs.ShowMessageBox(error, "ERROR");
    }

    public void OpenBatchDataFolder()
    {
      var batch = (IBatch) ExperimentNavigator.SelectedItem;
      Process.Start(batch.DataFolder);
    }

    public void RemoveSelected()
    {
      var batch = (IBatch) ExperimentNavigator.SelectedItem;
      var question = "Do you really want to delete the results? This can not be undone." + Environment.NewLine;
      question += "##Batch" + Environment.NewLine;
      question += batch + Environment.NewLine;
      question += "##Data folder" + Environment.NewLine;
      question += batch.DataFolder;
      Dialogs.ShowMessageBox(question, "DELETE", MessageBoxOptions.YesNo, DeleteBatch);
    }

    private void DeleteBatch(IMessageBox obj)
    {
      var batch = (IBatch) ExperimentNavigator.SelectedItem;
      if (obj.WasSelected(MessageBoxOptions.Yes))
      {
        try
        {
          _simulationService.DeleteBatch(batch);
        }
        catch (Exception exception)
        {
          var errorMessage = "Can not delete the data folder of the deleted batch:" + Environment.NewLine;
          errorMessage += "##Data folder" + Environment.NewLine;
          errorMessage += batch.DataFolder + Environment.NewLine;
          errorMessage += "##Cause" + Environment.NewLine;
          errorMessage += exception.Message + Environment.NewLine;
          errorMessage += "Please delete it manually.";
          Dialogs.ShowMessageBox(errorMessage, "ERROR");
        }
      }
    }

    private void OnProjectChanged()
    {
      SetCurrentProject();
    }

    private void SetCurrentProject()
    {
      ExperimentNavigator.Project = _store.Content;
    }

    private void UpdateToSelectedItem(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "SelectedItem" &&
          ExperimentNavigator.SelectedItem is IModel)
      {
        ExperimentControl.SelectedModel = (IModel) ExperimentNavigator.SelectedItem;
      }
      NotifyOfPropertyChange(() => CanOpenBatchDataFolder);
      NotifyOfPropertyChange(() => CanRemoveSelected);
    }

    public void HandleKeyInput(Key key)
    {
      
    }
  }
}