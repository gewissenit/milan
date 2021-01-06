using System.ComponentModel.Composition;
using Milan.Simulation;
using Milan.Simulation.CostAccounting;
using Milan.Simulation.CostAccounting.Factories;
using Milan.Simulation.Factories;
using Milan.Simulation.Observers;

namespace EcoFactory.Components.Factories
{
  [Export(typeof (ICostObserverFactory))][Export(typeof(ISimulationObserverFactory))]
  internal class WorkstationBlockedCostObserverFactory : ICostObserverFactory
  {
    public string Name
    {
      get { return "Blocked"; }
    }

    public bool CanHandle(IEntity entity)
    {
      return entity.GetType() == typeof (Workstation);
    }

    public ICostObserver Create()
    {
      return new CostObserverExtensions.Workstation.Blocked();
    }

    public ICostObserver Duplicate(ICostObserver materialObserver)
    {
      return Clone(materialObserver);
    }

    public bool CanHandle(ISimulationObserver simulationObserver)
    {
      return simulationObserver.GetType() == typeof (CostObserverExtensions.Workstation.Blocked);
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
      Milan.Simulation.Utils.PrepareEntityObserver((IEntityObserver) simulationObserver);
    }

    private ICostObserver Clone(ISimulationObserver costObserver)
    {
      var master = (CostObserverExtensions.Workstation.Blocked) costObserver;
      var clone = new CostObserverExtensions.Workstation.Blocked();
      Milan.Simulation.Utils.CloneEntityObserver(clone, master);
      Milan.Simulation.CostAccounting.Utils.CloneCostObserver(clone, master);
      Milan.Simulation.Utils.CloneTimeReferencedObserver(clone, master);
      return clone;
    }
  }
}