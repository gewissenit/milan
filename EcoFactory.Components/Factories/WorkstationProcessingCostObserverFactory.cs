using System.ComponentModel.Composition;
using Milan.Simulation;
using Milan.Simulation.CostAccounting;
using Milan.Simulation.CostAccounting.Factories;
using Milan.Simulation.Factories;
using Milan.Simulation.Observers;

namespace EcoFactory.Components.Factories
{
  [Export(typeof (ICostObserverFactory))][Export(typeof(ISimulationObserverFactory))]
  internal class WorkstationProcessingCostObserverFactory : ICostObserverFactory
  {
    public string Name
    {
      get { return "Processing"; }
    }

    public bool CanHandle(IEntity entity)
    {
      return entity.GetType() == typeof (Workstation);
    }

    public ICostObserver Create()
    {
      return new CostObserverExtensions.Workstation.Processing();
    }

    public ICostObserver Duplicate(ICostObserver materialObserver)
    {
      return Clone(materialObserver);
    }

    public bool CanHandle(ISimulationObserver simulationObserver)
    {
      return simulationObserver.GetType() == typeof (CostObserverExtensions.Workstation.Processing);
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
      var observer = (CostObserverExtensions.Workstation.Processing) simulationObserver;
      Milan.Simulation.Utils.PrepareEntityObserver(observer);
      Milan.Simulation.Utils.PrepareProductRelatedObserver(observer);
    }

    private ICostObserver Clone(ISimulationObserver costObserver)
    {
      var master = (CostObserverExtensions.Workstation.Processing) costObserver;
      var clone = new CostObserverExtensions.Workstation.Processing();
      Milan.Simulation.Utils.CloneEntityObserver(clone, master);
      Milan.Simulation.CostAccounting.Utils.CloneCostObserver(clone, master);
      Milan.Simulation.Utils.CloneProductRelatedObserver(clone, master);
      Milan.Simulation.Utils.CloneTimeReferencedObserver(clone, master);
      return clone;
    }
  }
}