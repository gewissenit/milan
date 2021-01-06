//#region License

//// Copyright (c) 2013 HTW Berlin
//// All rights reserved.

//#endregion

//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Linq;
//using System.Windows.Controls;
//using Caliburn.Micro;
//using Emporer.WPF.Commands;
//using Emporer.WPF.Factories;
//using Emporer.WPF.ViewModels;
//using Milan.Simulation.Factories;
//using Milan.Simulation.Observers;

//namespace Milan.Simulation.UI.ViewModels
//{
//  public class ExperimentEditViewModel : EditViewModel
//  {
//    private readonly IEnumerable<IEditViewModelFactory> _EditViewModelFactories;
//    private readonly ISimulationService _SimulationService;
//    private readonly IEnumerable<StatisticalObserverFactoryViewModel> _StatisticalObserverFactories;
//    private IHaveDisplayName _EditItem;

//    private int _NumberOfRuns = 1;
//    private IModel _SelectedModel;
//    private TimeSpan _SettlingTime = new TimeSpan(0);

//    public ExperimentEditViewModel(ISimulationService simulationService,
//                                    IEnumerable<IStatisticalObserverFactory> statisticalObserverFactories,
//                                    IEnumerable<IEditViewModelFactory> editViewModelFactories, ExperimentConfiguration configuration)
//      : base(configuration,"Experiment")
//    {
//      _EditViewModelFactories = editViewModelFactories;
//      DisplayName = "Experiment Control";

//      _SimulationService = simulationService;
//      _StatisticalObserverFactories = statisticalObserverFactories.Select(o => new StatisticalObserverFactoryViewModel(o, true)).Where(o=>configuration.StatisticalObservers.All(so => so.GetType() != o.Model.GetType())).Concat(configuration.StatisticalObservers.Select(o=>new StatisticalObserverFactoryViewModel(o, true)))
//                                                                   .ToArray();
//    }

//    public IEnumerable<StatisticalObserverFactoryViewModel> StatisticalObserverFactories
//    {
//      get { return _StatisticalObserverFactories; }
//    }

//    public void SelectAllStatisticalObserver()
//    {
//      foreach (var factory in StatisticalObserverFactories)
//      {
//        factory.IsSelected = true;
//      }
//    }

//    public void DeselectAllStatisticalObserver()
//    {
//      foreach (var factory in StatisticalObserverFactories)
//      {
//        factory.IsSelected = false;
//      }
//    }
//  }
//}