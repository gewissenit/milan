using System.ComponentModel.Composition;
using Milan.Simulation;
using Milan.Simulation.CostAccounting;
using Milan.Simulation.CostAccounting.Factories;
using Milan.Simulation.Factories;
using Milan.Simulation.Observers;

namespace EcoFactory.Components.Factories
{
  [Export(typeof (ICostObserverFactory))][Export(typeof(ISimulationObserverFactory))]
  internal class AssemblyStationSetupCostObserverFactory : ICostObserverFactory
  {
    public string Name
    {
      get { return "Setup"; }
    }

    public bool CanHandle(IEntity entity)
    {
      return entity.GetType() == typeof (AssemblyStation);
    }

    public ICostObserver Create()
    {
      return new CostObserverExtensions.AssemblyStation.Setup();
    }

    public ICostObserver Duplicate(ICostObserver materialObserver)
    {
      return Clone(materialObserver);
    }

    public bool CanHandle(ISimulationObserver simulationObserver)
    {
      return simulationObserver.GetType() == typeof (CostObserverExtensions.AssemblyStation.Setup);
    }

    public ISimulationObserver CreateSimulationObserver(ISimulationObserver simulationObserver, IExperiment experiment)
    {
      var clone = Clone(simulationObserver);
      clone.Id = simulationObserver.Id;
      clone.Configure(experiment);
      return clone;
    }

    public void Prepare(ISimulationObserver simulationObserver)
    {
      var observer = (CostObserverExtensions.AssemblyStation.Setup) simulationObserver;
      Milan.Simulation.Utils.PrepareEntityObserver(observer);
    }

    private ICostObserver Clone(ISimulationObserver costObserver)
    {
      var master = (CostObserverExtensions.AssemblyStation.Setup) costObserver;
      var clone = new CostObserverExtensions.AssemblyStation.Setup();
      Milan.Simulation.Utils.CloneEntityObserver(clone, master);
      Milan.Simulation.CostAccounting.Utils.CloneCostObserver(clone, master);
      Milan.Simulation.Utils.CloneTimeReferencedObserver(clone, master);
      return clone;
    }
  }
}