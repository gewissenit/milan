#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Caliburn.Micro;
using Emporer.WPF;
using Milan.JsonStore;
using Milan.Simulation.CostAccounting.UI.ViewModels;
using Milan.Simulation.MaterialAccounting.UI.ViewModels;
using Ork.Framework;

namespace Milan.UI.ViewModels
{
  [Export(typeof (IWorkspace))]
  public class FlowPerspectiveViewModel : Screen, IWorkspace
  {
    private readonly IJsonStore _store;

    [ImportingConstructor]
    public FlowPerspectiveViewModel([Import] IJsonStore store,
                                    [Import] FlowsNavigatorViewModel modelNavigatorViewModel,
                                    [Import] CostFlowsViewModel costFlowsViewModel,
                                    [Import] MaterialFlowsViewModel materialFlowsViewModel,
                                    [Import] ISelection flowPerspectiveSelection)
    {
      _store = store;
      Index = 30;
      DisplayName = "Flows";
      IsEnabled = true;

      CostFlows = costFlowsViewModel;
      CostFlows.Selection = flowPerspectiveSelection;

      MaterialFlows = materialFlowsViewModel;
      MaterialFlows.Selection = flowPerspectiveSelection;

      ModelNavigator = modelNavigatorViewModel;
      ModelNavigator.Selection = flowPerspectiveSelection;

      _store.ProjectChanged.Subscribe(_=> OnProjectChanged());

      SetCurrentProject();
    }

    public CostFlowsViewModel CostFlows { get; private set; }
    public MaterialFlowsViewModel MaterialFlows { get; private set; }
    public FlowsNavigatorViewModel ModelNavigator { get; private set; }

    public int Index { get; private set; }
    public bool IsEnabled { get; private set; }


    private void OnProjectChanged()
    {
      SetCurrentProject();
    }

    private void SetCurrentProject()
    {
      ModelNavigator.Project = _store.Content;
    }

    public void HandleKeyInput(Key key)
    {
      
    }
  }
}